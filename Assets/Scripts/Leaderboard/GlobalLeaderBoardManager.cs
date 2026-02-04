using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class GlobalLeaderBoardManager : MonoBehaviour
{
    public static GlobalLeaderBoardManager Instance { get; private set; } // Singleton

    // Replace "32898" with your actual leaderboard KEY from LootLocker dashboard
    // This is a string, not an integer (e.g., "quiz_leaderboard" or similar)
    string leaderboardKey = "32898";

    private void Awake()
    {
        // Singleton pattern - only one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    /// <summary>
    /// Submits a score to LootLocker's leaderboard
    /// Called automatically after quiz completion when player finishes 30 questions
    /// </summary>
    /// <param name="scoreToUpload">The final score to upload</param>
    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        
        // Check if player is authenticated with LootLocker
        if (string.IsNullOrEmpty(playerID))
        {
            Debug.LogWarning("Cannot submit score: Player not logged in to LootLocker");
            yield break;
        }
        
        Debug.Log($"Submitting score to LootLocker: {scoreToUpload} for PlayerID: {playerID}");
        
        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardKey, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully uploaded score to LootLocker leaderboard!");
                done = true;
            }
            else
            {
                Debug.Log("Failed to upload score: " + response.errorData.message);
                done = true;
            }
        });
        
        yield return new WaitWhile(() => done == false);
    }
}
