// SCRIPT 3: PLAYER MANAGER - Saves player information
// This script stores the player's name, school, and final score
// It saves this information to a JSON file on the device locally
// When they finish the quiz, we can access this to show leaderboards later
// Also handles LootLocker authentication for online leaderboards

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using LootLocker.Requests;

[System.Serializable]
public class PlayerRecord
{
    public string firstName;
    public string lastName;
    public string school;
    public int finalScore;
    public string timestamp; // Format: MM/DD/YYYY
}

[System.Serializable]
public class PlayerDatabase
{
    public List<PlayerRecord> allRecords = new List<PlayerRecord>();
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; } // Singleton - one instance everywhere

    [Header("=== PLAYER INFO (SET BY ONBOARDING) ===")]
    private string playerFirstName = "";
    private string playerLastName = "";
    private string playerSchool = "";
    private int playerFinalScore = 0;

    [Header("=== LOOTLOCKER INFO ===")]
    private string lootLockerPlayerID = "";
    private bool isLootLockerAuthenticated = false;

    private string savePath;

    private void Awake()
    {
        // Singleton pattern - only one PlayerManager in the whole game
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this object when scenes change

        // Set up the save file path
        savePath = Path.Combine(Application.persistentDataPath, "player_scores.json");
    }

    private void Start()
    {
        // Authenticate with LootLocker when the game starts
        StartCoroutine(LoginRoutine());
    }

    // === LOOTLOCKER AUTHENTICATION ===
    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if(response.success)
            {
                Debug.Log("Player was logged in to LootLocker");
                lootLockerPlayerID = response.player_id.ToString();
                PlayerPrefs.SetString("PlayerID", lootLockerPlayerID);
                isLootLockerAuthenticated = true;
                done = true;
            }
            else
            {
                Debug.Log("Could not start LootLocker session: " + response.errorData.message);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    // Get LootLocker player ID
    public string GetLootLockerPlayerID() => lootLockerPlayerID;
    public bool IsLootLockerAuthenticated() => isLootLockerAuthenticated;

    // Called by UIManager when the player enters their info
    public void SetPlayerInfo(string firstName, string lastName, string school)
    {
        playerFirstName = firstName;
        playerLastName = lastName;
        playerSchool = school;
        Debug.Log($"Player set: {firstName} {lastName} from {school}");
        
        // Send player info to LootLocker if authenticated
        if (isLootLockerAuthenticated && GlobalLeaderBoardManager.Instance != null)
        {
            // Set player name on LootLocker (visible on leaderboards)
            StartCoroutine(GlobalLeaderBoardManager.Instance.SetPlayerNameRoutine(firstName, lastName));
            
            // Save additional metadata (firstName, lastName, school)
            GlobalLeaderBoardManager.Instance.SavePlayerMetadata(firstName, lastName, school);
        }
        else
        {
            if (!isLootLockerAuthenticated)
                Debug.LogWarning("Player not authenticated with LootLocker yet. Player info will be saved locally only.");
            if (GlobalLeaderBoardManager.Instance == null)
                Debug.LogWarning("GlobalLeaderBoardManager not found. Player info will be saved locally only.");
        }
    }

    // Called by QuizProper when the quiz finishes
    public void SetFinalScore(int score)
    {
        playerFinalScore = score;
        SavePlayerRecord();
    }

    // Save the player's record to a JSON file on the device
    private void SavePlayerRecord()
    {
        // Load existing records
        PlayerDatabase database = LoadDatabase();

        // Create a new record
        PlayerRecord newRecord = new PlayerRecord
        {
            firstName = playerFirstName,
            lastName = playerLastName,
            school = playerSchool,
            finalScore = playerFinalScore,
            timestamp = System.DateTime.Now.ToString("MM/dd/yyyy")
        };

        // Add the new record
        database.allRecords.Add(newRecord);

        // Save back to file
        string json = JsonUtility.ToJson(database, true); // 'true' means pretty print (readable)
        File.WriteAllText(savePath, json);

        Debug.Log("Player record saved to: " + savePath);
    }

    // Load all player records from the file
    public PlayerDatabase LoadDatabase()
    {
        if (!File.Exists(savePath))
        {
            return new PlayerDatabase(); // Empty database if file doesn't exist
        }

        string json = File.ReadAllText(savePath);
        PlayerDatabase database = JsonUtility.FromJson<PlayerDatabase>(json);
        return database ?? new PlayerDatabase();
    }

    // Get the top 5 scores locally (for local leaderboard)
    public List<PlayerRecord> GetTopLocalScores(int count = 5)
    {
        PlayerDatabase database = LoadDatabase();
        
        // Sort by score descending and take top 5
        List<PlayerRecord> sorted = database.allRecords;
        sorted.Sort((a, b) => b.finalScore.CompareTo(a.finalScore)); // Sort highest first
        
        return sorted.Take(count).ToList();
    }

    // Get top 5 scores for a specific school
    public List<PlayerRecord> GetTopScoresForSchool(string schoolName, int count = 5)
    {
        PlayerDatabase database = LoadDatabase();
        
        // Filter by school and sort
        List<PlayerRecord> filtered = database.allRecords.FindAll(r => r.school == schoolName);
        filtered.Sort((a, b) => b.finalScore.CompareTo(a.finalScore));
        
        return filtered.Take(count).ToList();
    }

    // Get the 5 most recent records (for local leaderboard)
    public List<PlayerRecord> GetRecentRecords(int count = 5)
    {
        PlayerDatabase database = LoadDatabase();
        
        // Get records in reverse order (newest first)
        // Since we always add new records at the end, we take the last N records
        int totalRecords = database.allRecords.Count;
        int startIndex = Mathf.Max(0, totalRecords - count);
        
        List<PlayerRecord> recentRecords = new List<PlayerRecord>();
        for (int i = totalRecords - 1; i >= startIndex; i--)
        {
            recentRecords.Add(database.allRecords[i]);
        }
        
        return recentRecords;
    }

    // Clear all records (for debugging/testing purposes)
    public void ClearAllRecords()
    {
        PlayerDatabase emptyDatabase = new PlayerDatabase();
        string json = JsonUtility.ToJson(emptyDatabase, true);
        File.WriteAllText(savePath, json);
        Debug.Log("All player records cleared.");
    }

    // Get the current player's first name
    public string GetPlayerFirstName() => playerFirstName;
    public string GetPlayerLastName() => playerLastName;
    public string GetPlayerSchool() => playerSchool;
    public int GetPlayerFinalScore() => playerFinalScore;
}
