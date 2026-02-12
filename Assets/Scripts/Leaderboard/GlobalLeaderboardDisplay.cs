using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

public class GlobalLeaderboardDisplay : MonoBehaviour
{
    [Header("=== LEADERBOARD DISPLAY ===")]
    [Tooltip("These TextMesh Pro fields will display the top 5 players")]
    [SerializeField] private TextMeshProUGUI[] rankTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] schoolTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[5];

    [Header("=== STATUS TEXT ===")]
    [SerializeField] private TextMeshProUGUI emptyStateText;
    [SerializeField] private string noScoresMessage = "NO HIGH SCORES YET. BE THE FIRST TO PLAY!";
    [SerializeField] private string topScoresMessage = "These are the top {0} scores.";
    [SerializeField] private string topScoresWithSlotsMessage = "These are the top {0} scores, there are {1} slots left.";

    [Header("=== NAVIGATION BUTTONS ===")]
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button refreshLeaderboardButton;

    [Header("=== SCENE NAMES ===")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("=== UGS LEADERBOARD ===")]
    [SerializeField] private string leaderboardId = "FTICSDGQuest";

    private const int MaxEntries = 5;

    [Serializable]
    private class LeaderboardMetadata
    {
        public string name;
        public string school;
    }

    private async void Start()
    {
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(GoToMainMenu);

        if (refreshLeaderboardButton != null)
            refreshLeaderboardButton.onClick.AddListener(RefreshLeaderboard);

        await LoadAndDisplayLeaderboardAsync();
    }

    public async void RefreshLeaderboard()
    {
        await LoadAndDisplayLeaderboardAsync();
    }

    private async Task LoadAndDisplayLeaderboardAsync()
    {
        string resolvedLeaderboardId = leaderboardId;
        if (string.IsNullOrWhiteSpace(resolvedLeaderboardId) && PlayerManager.Instance != null)
        {
            resolvedLeaderboardId = PlayerManager.Instance.GetLeaderboardId();
        }

        if (string.IsNullOrWhiteSpace(resolvedLeaderboardId))
        {
            Debug.LogError("Leaderboard ID is missing. Please set it in the Inspector.");
            ShowEmptyState();
            return;
        }

        try
        {
            await EnsureSignedInAsync();

            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                resolvedLeaderboardId,
                new GetScoresOptions { Offset = 0, Limit = MaxEntries, IncludeMetadata = true });

            List<LeaderboardEntry> results = scoresResponse?.Results;
            if (results == null || results.Count == 0)
            {
                ShowEmptyState();
                return;
            }

            UpdateStatusText(results.Count);

            for (int i = 0; i < MaxEntries; i++)
            {
                if (i < results.Count)
                {
                    var entry = results[i];
                    var meta = ParseMetadata(entry.Metadata);
                    string displayName = GetDisplayName(entry, meta);

                    if (rankTexts[i] != null)
                        rankTexts[i].text = $"#{entry.Rank + 1}";

                    if (nameTexts[i] != null)
                    {
                        int nameLength = displayName.Length;

                        if (nameLength > 10)
                        {
                            nameTexts[i].fontSize = 105;
                            displayName = displayName.Replace(" ", "\n");
                        }
                        else if (nameLength > 9)
                        {
                            nameTexts[i].fontSize = 130;
                        }
                        else
                        {
                            nameTexts[i].fontSize = 150;
                        }

                        nameTexts[i].text = displayName;
                    }

                    if (schoolTexts[i] != null)
                        schoolTexts[i].text = meta != null && !string.IsNullOrEmpty(meta.school) ? meta.school : "-";

                    if (scoreTexts[i] != null)
                        scoreTexts[i].text = ((int)entry.Score).ToString();

                    SetRowActive(i, true);
                }
                else
                {
                    SetRowActive(i, false);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Global leaderboard load failed: {ex.Message}");
            ShowEmptyState();
        }
    }

    private async Task EnsureSignedInAsync()
    {
        if (PlayerManager.Instance != null)
        {
            await PlayerManager.Instance.EnsureSignedInAsync();
            return;
        }

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private LeaderboardMetadata ParseMetadata(string metadataJson)
    {
        if (string.IsNullOrEmpty(metadataJson))
            return null;

        try
        {
            return JsonUtility.FromJson<LeaderboardMetadata>(metadataJson);
        }
        catch
        {
            return null;
        }
    }

    private string GetDisplayName(LeaderboardEntry entry, LeaderboardMetadata meta)
    {
        if (meta != null && !string.IsNullOrEmpty(meta.name))
        {
            string fullName = meta.name;
            string[] nameParts = fullName.Split(' ');
            
            if (nameParts.Length >= 2)
            {
                string firstName = nameParts[0];
                string lastName = nameParts[nameParts.Length - 1];
                return firstName + " " + lastName[0] + ".";
            }
            
            return fullName;
        }

        if (!string.IsNullOrEmpty(entry.PlayerName))
            return entry.PlayerName;

        return "Player";
    }

    private void SetRowActive(int index, bool isActive)
    {
        if (rankTexts[index] != null) rankTexts[index].gameObject.SetActive(isActive);
        if (nameTexts[index] != null) nameTexts[index].gameObject.SetActive(isActive);
        if (schoolTexts[index] != null) schoolTexts[index].gameObject.SetActive(isActive);
        if (scoreTexts[index] != null) scoreTexts[index].gameObject.SetActive(isActive);
    }

    private void ShowEmptyState()
    {
        for (int i = 0; i < MaxEntries; i++)
        {
            SetRowActive(i, false);
        }

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

        if (scoreCount <= 0)
        {
            emptyStateText.text = noScoresMessage;
        }
        else if (scoreCount >= MaxEntries)
        {
            emptyStateText.text = string.Format(topScoresMessage, MaxEntries);
        }
        else
        {
            int slotsLeft = MaxEntries - scoreCount;
            emptyStateText.text = string.Format(topScoresWithSlotsMessage, scoreCount, slotsLeft);
        }

        emptyStateText.gameObject.SetActive(true);
    }

    private void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
}
