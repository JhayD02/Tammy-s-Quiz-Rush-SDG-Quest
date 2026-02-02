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
    [SerializeField] private TextMeshProUGUI[] lastNameTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] schoolTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] dateTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[5];

    [Header("=== OPTIONAL: CURRENT PLAYER HIGHLIGHT ===")]
    [Tooltip("If you want to show the current player's info separately")]
    [SerializeField] private GameObject currentPlayerPanel;
    [SerializeField] private TextMeshProUGUI currentPlayerNameText;
    [SerializeField] private TextMeshProUGUI currentPlayerLastNameText;
    [SerializeField] private TextMeshProUGUI currentPlayerSchoolText;
    [SerializeField] private TextMeshProUGUI currentPlayerDateText;
    [SerializeField] private TextMeshProUGUI currentPlayerScoreText;

    [Header("=== EMPTY STATE TEXT ===")]
    [SerializeField] private TextMeshProUGUI emptyStateText;
    [SerializeField] private string emptyMessage = "No quiz sessions yet. Play to create history!";

    [Header("=== NAVIGATION BUTTONS ===")]
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button playAgainButton;

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

        // Load and display the leaderboard
        LoadAndDisplayLeaderboard();
    }

    private void LoadAndDisplayLeaderboard()
    {
        // Get the top 5 highest scores from PlayerManager
        List<PlayerRecord> topScores = PlayerManager.Instance.GetTopLocalScores(5);

        // Check if we have any records
        if (topScores == null || topScores.Count == 0)
        {
            ShowEmptyState();
            return;
        }

        // Hide empty state message
        if (emptyStateText != null)
            emptyStateText.gameObject.SetActive(false);

        // Display current player info if panel exists
        if (currentPlayerPanel != null)
        {
            DisplayCurrentPlayer();
        }

        // Display the top scores (sorted from highest to lowest)
        for (int i = 0; i < 5; i++)
        {
            if (i < topScores.Count)
            {
                // We have a record for this slot
                PlayerRecord record = topScores[i];
                
                // Display name (First + Last)
                if (nameTexts[i] != null)
                    nameTexts[i].text = record.firstName;
                
                // Display last name
                if (lastNameTexts[i] != null)
                    lastNameTexts[i].text = record.lastName;
                
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
                if (lastNameTexts[i] != null) lastNameTexts[i].gameObject.SetActive(true);
                if (schoolTexts[i] != null) schoolTexts[i].gameObject.SetActive(true);
                if (dateTexts[i] != null) dateTexts[i].gameObject.SetActive(true);
                if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(true);
            }
            else
            {
                // No record for this slot, hide the texts
                if (nameTexts[i] != null) nameTexts[i].gameObject.SetActive(false);
                if (lastNameTexts[i] != null) lastNameTexts[i].gameObject.SetActive(false);
                if (schoolTexts[i] != null) schoolTexts[i].gameObject.SetActive(false);
                if (dateTexts[i] != null) dateTexts[i].gameObject.SetActive(false);
                if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(false);
            }
        }
    }

    private void DisplayCurrentPlayer()
    {
        // Show the most recent player (current player) in a highlighted panel
        if (PlayerManager.Instance == null)
        {
            currentPlayerPanel.SetActive(false);
            return;
        }

        string firstName = PlayerManager.Instance.GetPlayerFirstName();
        string lastName = PlayerManager.Instance.GetPlayerLastName();
        string school = PlayerManager.Instance.GetPlayerSchool();
        int score = PlayerManager.Instance.GetPlayerFinalScore();
        string date = System.DateTime.Now.ToString("MM/dd/yyyy");

        // Check if we have valid player data
        if (string.IsNullOrEmpty(firstName))
        {
            currentPlayerPanel.SetActive(false);
            return;
        }

        // Display in the current player panel
        currentPlayerPanel.SetActive(true);
        
        if (currentPlayerNameText != null)
            currentPlayerNameText.text = firstName;
        
        if (currentPlayerLastNameText != null)
            currentPlayerLastNameText.text = lastName;
        
        if (currentPlayerSchoolText != null)
            currentPlayerSchoolText.text = school;
        
        if (currentPlayerDateText != null)
            currentPlayerDateText.text = date;
        
        if (currentPlayerScoreText != null)
            currentPlayerScoreText.text = score.ToString();
    }

    private void ShowEmptyState()
    {
        // Hide all leaderboard entries
        for (int i = 0; i < 5; i++)
        {
            if (nameTexts[i] != null) nameTexts[i].gameObject.SetActive(false);
            if (lastNameTexts[i] != null) lastNameTexts[i].gameObject.SetActive(false);
            if (schoolTexts[i] != null) schoolTexts[i].gameObject.SetActive(false);
            if (dateTexts[i] != null) dateTexts[i].gameObject.SetActive(false);
            if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(false);
        }

        // Show empty state message
        if (emptyStateText != null)
        {
            emptyStateText.text = emptyMessage;
            emptyStateText.gameObject.SetActive(true);
        }

        // Hide current player panel if it exists
        if (currentPlayerPanel != null)
            currentPlayerPanel.SetActive(false);
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
    public void RefreshLeaderboard()
    {
        LoadAndDisplayLeaderboard();
    }

    public void ClearAllRecords()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ClearAllRecords();
            LoadAndDisplayLeaderboard();
        }
    }
}
