// LOCAL LEADERBOARD MANAGER - TOP 5 SCORES
// This script displays the TOP 5 HIGHEST SCORES from all quiz sessions
// Shows: Rank, First Name + Last Name, School, and Score
// Designed for TextMesh Pro text display
// Future ready for: Global leaderboard and School-specific leaderboard

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocalLeaderboardManager : MonoBehaviour
{
    [Header("=== TOP 5 LEADERBOARD DISPLAY ===")]
    [Tooltip("Drag 5 TextMesh Pro text fields to display FIRST NAMES")]
    [SerializeField] private TextMeshProUGUI[] firstNameTexts = new TextMeshProUGUI[5];
    
    [Tooltip("Drag 5 TextMesh Pro text fields to display LAST NAMES")]
    [SerializeField] private TextMeshProUGUI[] lastNameTexts = new TextMeshProUGUI[5];

    [Header("=== LAST NAME FONT SETTINGS ===")]
    [Tooltip("Base font size for last names")]
    [SerializeField] private float lastNameBaseFontSize = 120f;
    [Tooltip("If last name length exceeds this, reduce font size")]
    [SerializeField] private int lastNameLengthThreshold = 12;
    [Tooltip("Font size reduction when last name is too long")]
    [SerializeField] private float lastNameFontSizeReduction = 15f;
    
    [Tooltip("Drag 5 TextMesh Pro text fields to display schools")]
    [SerializeField] private TextMeshProUGUI[] schoolTexts = new TextMeshProUGUI[5];
    
    [Tooltip("Drag 5 TextMesh Pro text fields to display scores")]
    [SerializeField] private TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[5];

    [Header("=== EMPTY STATE MESSAGE ===")]
    [Tooltip("Shows this message when no scores exist yet")]
    [SerializeField] private TextMeshProUGUI emptyStateText;
    [SerializeField] private string emptyMessage = "No high scores yet. Be the first to play!";

    [Header("=== STATUS MESSAGE (WHEN DATA EXISTS) ===")]
    [Tooltip("Use {count} for number of scores, {scoreWord} for score/scores, {slots} for remaining slots, {slotWord} for slot/slots")]
    [SerializeField] private string statusMessage = "Here are the top {count} {scoreWord}, {slots} {slotWord} left!";


    [Header("=== NAVIGATION BUTTONS ===")]
    [Tooltip("Button to return to main menu")]
    [SerializeField] private Button backToMenuButton;
    
    [Tooltip("Button to play the quiz again")]
    [SerializeField] private Button playAgainButton;

    [Header("=== SCENE NAMES ===")]
    [Tooltip("Name of your main menu scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    [Tooltip("Name of your quiz scene")]
    [SerializeField] private string quizSceneName = "Quiz Scene Proper";

    // NOTE FOR FUTURE DEVELOPMENT:
    // - For GLOBAL leaderboard: Create online database integration
    // - For SCHOOL leaderboard: Use GetTopScoresForSchool(schoolName, 5) method
    // - For RECENT HISTORY: Use GetRecentRecords(5) method
    // These methods are already built in PlayerManager but not currently used

    private void Start()
    {
        // Set up button listeners
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        // Load and display the top 5 scores
        LoadAndDisplayTopScores();
    }

    /// <summary>
    /// Loads the TOP 5 HIGHEST SCORES from local storage and displays them
    /// </summary>
    private void LoadAndDisplayTopScores()
    {
        // Safety check: Make sure PlayerManager exists
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found! Make sure it exists in your scene.");
            ShowEmptyState();
            return;
        }

        // Get the TOP 5 highest scores from PlayerManager
        List<PlayerRecord> topScores = PlayerManager.Instance.GetTopLocalScores(5);

        // Check if we have any records
        if (topScores == null || topScores.Count == 0)
        {
            ShowEmptyState();
            return;
        }

        // Show status message using the same UI element
        if (emptyStateText != null)
        {
            int shownCount = topScores.Count;
            int slotsLeft = Mathf.Max(0, 5 - shownCount);
            string slotsText = slotsLeft == 1 ? "slot" : "slots";
            string scoreText = shownCount == 1 ? "score" : "scores";
            string formattedMessage = statusMessage
                .Replace("{count}", shownCount.ToString())
                .Replace("{scoreWord}", scoreText)
                .Replace("{slots}", slotsLeft.ToString())
                .Replace("{slotWord}", slotsText);

            if (!statusMessage.Contains("{scoreWord}") && shownCount == 1)
            {
                formattedMessage = formattedMessage.Replace("scores", "score");
            }

            emptyStateText.text = formattedMessage;
            emptyStateText.gameObject.SetActive(true);
        }

        // Display the top 5 scores
        for (int i = 0; i < 5; i++)
        {
            if (i < topScores.Count)
            {
                // We have a score for this rank
                PlayerRecord record = topScores[i];
                
                // Display first name
                if (firstNameTexts[i] != null)
                {
                    firstNameTexts[i].text = record.firstName;
                    firstNameTexts[i].gameObject.SetActive(true);
                }
                
                // Display last name
                if (lastNameTexts[i] != null)
                {
                    lastNameTexts[i].text = record.lastName;

                    int lastNameLength = string.IsNullOrEmpty(record.lastName) ? 0 : record.lastName.Length;
                    float targetFontSize = lastNameBaseFontSize;

                    if (lastNameLength > lastNameLengthThreshold)
                    {
                        targetFontSize -= lastNameFontSizeReduction;
                    }

                    if (lastNameLength > 15)
                    {
                        targetFontSize -= lastNameFontSizeReduction;
                    }

                    lastNameTexts[i].fontSize = targetFontSize;

                    lastNameTexts[i].gameObject.SetActive(true);
                }
                
                // Display school
                if (schoolTexts[i] != null)
                {
                    schoolTexts[i].text = record.school;
                    schoolTexts[i].gameObject.SetActive(true);
                }
                
                // Display score
                if (scoreTexts[i] != null)
                {
                    scoreTexts[i].text = record.finalScore.ToString();
                    scoreTexts[i].gameObject.SetActive(true);
                }
            }
            else
            {
                // No score for this rank, hide the UI elements
                if (firstNameTexts[i] != null) 
                    firstNameTexts[i].gameObject.SetActive(false);
                
                if (lastNameTexts[i] != null) 
                    lastNameTexts[i].gameObject.SetActive(false);
                    
                if (schoolTexts[i] != null) 
                    schoolTexts[i].gameObject.SetActive(false);
                    
                if (scoreTexts[i] != null) 
                    scoreTexts[i].gameObject.SetActive(false);
            }
        }

        Debug.Log($"Leaderboard loaded: {topScores.Count} scores displayed");
    }


    /// <summary>
    /// Shows empty state when no scores exist yet
    /// </summary>
    private void ShowEmptyState()
    {
        // Hide all leaderboard entry UI elements
        for (int i = 0; i < 5; i++)
        {
            if (firstNameTexts[i] != null) 
                firstNameTexts[i].gameObject.SetActive(false);
            
            if (lastNameTexts[i] != null) 
                lastNameTexts[i].gameObject.SetActive(false);
                
            if (schoolTexts[i] != null) 
                schoolTexts[i].gameObject.SetActive(false);
                
            if (scoreTexts[i] != null) 
                scoreTexts[i].gameObject.SetActive(false);
        }

        // Show the empty state message
        if (emptyStateText != null)
        {
            emptyStateText.text = emptyMessage;
            emptyStateText.gameObject.SetActive(true);
        }

        Debug.Log("Leaderboard is empty - no scores yet.");
    }

    // === NAVIGATION METHODS ===
    
    /// <summary>
    /// Returns to the main menu scene
    /// </summary>
    private void GoToMainMenu()
    {
        Debug.Log($"Loading scene: {mainMenuSceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Restarts the quiz for a new play session
    /// </summary>
    private void PlayAgain()
    {
        Debug.Log($"Loading scene: {quizSceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(quizSceneName);
    }

    // === PUBLIC METHODS FOR RUNTIME USE ===
    
    /// <summary>
    /// Call this to manually refresh the leaderboard display
    /// Useful if you dynamically load new scores
    /// </summary>
    public void RefreshLeaderboard()
    {
        LoadAndDisplayTopScores();
        Debug.Log("Leaderboard manually refreshed");
    }

    /// <summary>
    /// Clears all saved records (useful for testing/debugging)
    /// WARNING: This deletes ALL player data permanently!
    /// Works in both Play Mode and Edit Mode
    /// </summary>
    public void ClearAllRecords()
    {
        // Try using PlayerManager.Instance first (if in Play Mode)
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ClearAllRecords();
            LoadAndDisplayTopScores();
            Debug.Log("✓ All records cleared via PlayerManager and leaderboard refreshed");
        }
        else
        {
            // If not in Play Mode, directly clear the JSON file
            string savePath = Path.Combine(Application.persistentDataPath, "player_scores.json");
            
            if (File.Exists(savePath))
            {
                // Create empty database
                PlayerDatabase emptyDatabase = new PlayerDatabase();
                string json = JsonUtility.ToJson(emptyDatabase, true);
                File.WriteAllText(savePath, json);
                
                Debug.Log("✓ All records cleared directly from file: " + savePath);
                
                // Refresh display if we're in Play Mode
                if (Application.isPlaying)
                {
                    LoadAndDisplayTopScores();
                }
            }
            else
            {
                Debug.LogWarning("No save file found at: " + savePath);
            }
        }
    }
}

// === CUSTOM INSPECTOR EDITOR (Only in Unity Editor) ===
#if UNITY_EDITOR
[CustomEditor(typeof(LocalLeaderboardManager))]
public class LocalLeaderboardManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (all the serialized fields)
        DrawDefaultInspector();
        
        LocalLeaderboardManager manager = (LocalLeaderboardManager)target;

        // Add spacing
        EditorGUILayout.Space(15);
        
        // Add a header
        EditorGUILayout.LabelField("=== TESTING & DEBUG TOOLS ===", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("These buttons only work in the Unity Editor for testing.", MessageType.Info);
        
        EditorGUILayout.Space(5);

        // Refresh button (safe, non-destructive)
        if (GUILayout.Button("🔄 Refresh Leaderboard Display", GUILayout.Height(35)))
        {
            manager.RefreshLeaderboard();
        }

        EditorGUILayout.Space(5);

        // Clear all records button (destructive, needs confirmation)
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("⚠️ CLEAR ALL RECORDS", GUILayout.Height(35)))
        {
            // Show confirmation dialog
            if (EditorUtility.DisplayDialog(
                "Clear All Leaderboard Records?", 
                "This will permanently delete ALL saved player scores from local storage.\n\n" +
                "This action cannot be undone!\n\n" +
                "Are you sure you want to continue?", 
                "Yes, Delete Everything", 
                "Cancel"))
            {
                manager.ClearAllRecords();
                EditorUtility.DisplayDialog("Success", "All records have been cleared!", "OK");
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);
    }
}
#endif
