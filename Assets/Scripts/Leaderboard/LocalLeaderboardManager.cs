// LOCAL LEADERBOARD MANAGER
// This script displays the 5 most recent quiz sessions
// Shows: First Name + Last Name, School, Date (MM/DD/YYYY), and Score
// Designed for TextMesh Pro text display

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LocalLeaderboardManager : MonoBehaviour
{
    [Header("=== LEADERBOARD DISPLAY ===")]
    [Tooltip("These TextMesh Pro fields will display the 5 most recent players")]
    [SerializeField] private TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] schoolTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] dateTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[5];

    [Header("=== STATUS TEXT ===")]
    [SerializeField] private TextMeshProUGUI emptyStateText;
    [SerializeField] private string noScoresMessage = "There are no current scores.";
    [SerializeField] private string topScoresMessage = "These are the top {0} scores.";
    [SerializeField] private string topScoresWithSlotsMessage = "These are the top {0} scores, there are {1} slots left.";

    [Header("=== NAVIGATION BUTTONS ===")]
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button playAgainButton;

    [Header("=== DEBUG BUTTONS ===")]
    [SerializeField] private Button refreshLeaderboardButton;
    [SerializeField] private Button clearAllRecordsButton;

    [Header("=== SCENE NAMES ===")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string quizSceneName = "Quiz Scene Proper";

    private void Start()
    {
        // Set up button listeners
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (refreshLeaderboardButton != null)
            refreshLeaderboardButton.onClick.AddListener(RefreshLeaderboard);

        if (clearAllRecordsButton != null)
            clearAllRecordsButton.onClick.AddListener(ClearAllRecords);

        // Load and display the leaderboard
        LoadAndDisplayLeaderboard();
    }

    private void LoadAndDisplayLeaderboard()
    {
        // Check if PlayerManager exists
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager.Instance is null! Make sure you start from Main Menu scene where PlayerManager is created.");
            ShowEmptyState();
            return;
        }

        // Get the top 5 highest scores from PlayerManager
        List<PlayerRecord> topScores = PlayerManager.Instance.GetTopLocalScores(5);

        // Check if we have any records
        if (topScores == null || topScores.Count == 0)
        {
            ShowEmptyState();
            return;
        }

        // Update status text
        UpdateStatusText(topScores.Count);

        // Display the top scores (sorted from highest to lowest)
        for (int i = 0; i < 5; i++)
        {
            if (i < topScores.Count)
            {
                // We have a record for this slot
                PlayerRecord record = topScores[i];
                
                // Format name as "FirstName LastInitial." (e.g., "James D.")
                string displayName = record.firstName;
                if (!string.IsNullOrEmpty(record.lastName))
                {
                    displayName += " " + record.lastName[0] + ".";
                }
                
                if (nameTexts[i] != null)
                {
                    int nameLength = displayName.Length;
                    int baseFontSize = 120;
                    int minFontSize = 40;
                    int fontSize = Mathf.Max(minFontSize, baseFontSize - (nameLength - 1) * 2);

                    nameTexts[i].fontSize = fontSize;
                    nameTexts[i].text = displayName;
                }
                
                // Display school
                if (schoolTexts[i] != null)
                    schoolTexts[i].text = record.school;
                
                // Display date
                if (dateTexts[i] != null)
                    dateTexts[i].text = record.timestamp;
                
                // Display score
                if (scoreTexts[i] != null)
                    scoreTexts[i].text = record.finalScore.ToString();

                // Make sure all are visible
                if (nameTexts[i] != null) nameTexts[i].gameObject.SetActive(true);
                if (schoolTexts[i] != null) schoolTexts[i].gameObject.SetActive(true);
                if (dateTexts[i] != null) dateTexts[i].gameObject.SetActive(true);
                if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(true);
            }
            else
            {
                // No record for this slot, hide the texts
                if (nameTexts[i] != null) nameTexts[i].gameObject.SetActive(false);
                if (schoolTexts[i] != null) schoolTexts[i].gameObject.SetActive(false);
                if (dateTexts[i] != null) dateTexts[i].gameObject.SetActive(false);
                if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowEmptyState()
    {
        // Hide all leaderboard entries
        for (int i = 0; i < 5; i++)
        {
            if (nameTexts[i] != null) nameTexts[i].gameObject.SetActive(false);
            if (schoolTexts[i] != null) schoolTexts[i].gameObject.SetActive(false);
            if (dateTexts[i] != null) dateTexts[i].gameObject.SetActive(false);
            if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(false);
        }

        // Show empty state message
        if (emptyStateText != null)
        {
            emptyStateText.text = noScoresMessage;
            emptyStateText.gameObject.SetActive(true);
        }
    }

    private void UpdateStatusText(int scoreCount)
    {
        if (emptyStateText == null)
            return;

        int maxSlots = 5;
        if (scoreCount <= 0)
        {
            emptyStateText.text = noScoresMessage;
        }
        else if (scoreCount >= maxSlots)
        {
            emptyStateText.text = string.Format(topScoresMessage, maxSlots);
        }
        else
        {
            int slotsLeft = maxSlots - scoreCount;
            emptyStateText.text = string.Format(topScoresWithSlotsMessage, scoreCount, slotsLeft);
        }

        emptyStateText.gameObject.SetActive(true);
    }

    // === NAVIGATION ===
    private void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    private void PlayAgain()
    {
        // Reset player info for new session
        // Note: PlayerManager persists, but we can start fresh
        UnityEngine.SceneManagement.SceneManager.LoadScene(quizSceneName);
    }

    // === PUBLIC METHODS FOR DEBUGGING ===
    [ContextMenu("Refresh Leaderboard")]
    public void RefreshLeaderboard()
    {
        LoadAndDisplayLeaderboard();
    }

    [ContextMenu("Clear All Records (JSON)")]
    public void ClearAllRecords()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ClearAllRecords();
            LoadAndDisplayLeaderboard();
            Debug.Log("All records cleared successfully!");
        }
        else
        {
            Debug.LogError("Cannot clear records: PlayerManager.Instance is null! Make sure you start from Main Menu scene.");
        }
    }
}
