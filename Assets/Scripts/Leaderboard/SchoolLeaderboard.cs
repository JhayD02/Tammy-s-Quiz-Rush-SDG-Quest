// SCHOOL LEADERBOARD - Displays top 5 scores from each FEU school + others worldwide
// This script filters the global UGS leaderboard by school metadata
// Each school has its own panel with top 5 scorers from that school

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

public class SchoolLeaderboard : MonoBehaviour
{
    [Header("=== SCHOOL BUTTONS ===")]
    [SerializeField] private Button feuTechButton;
    [SerializeField] private Button feuDilimanButton;
    [SerializeField] private Button feuAlabanButton;
    [SerializeField] private Button othersButton;

    [Header("=== SCHOOL PANELS ===")]
    [SerializeField] private GameObject feuTechPanel;
    [SerializeField] private GameObject feuDilimanPanel;
    [SerializeField] private GameObject feuAlabanPanel;
    [SerializeField] private GameObject othersPanel;

    [Header("=== FEU-TECH LEADERBOARD ===")]
    [SerializeField] private TextMeshProUGUI[] techNameTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] techSchoolTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] techScoreTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI techStatusText;

    [Header("=== FEU-DILIMAN LEADERBOARD ===")]
    [SerializeField] private TextMeshProUGUI[] dilimanNameTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] dilimanSchoolTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] dilimanScoreTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI dilimanStatusText;

    [Header("=== FEU-ALABANG LEADERBOARD ===")]
    [SerializeField] private TextMeshProUGUI[] alabangNameTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] alabangSchoolTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] alabangScoreTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI alabangStatusText;

    [Header("=== OTHERS LEADERBOARD ===")]
    [SerializeField] private TextMeshProUGUI[] othersNameTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] othersSchoolTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI[] othersScoreTexts = new TextMeshProUGUI[5];
    [SerializeField] private TextMeshProUGUI othersStatusText;

    [Header("=== STATUS MESSAGES ===")]
    [SerializeField] private string noScoresMessage = "NO SCORES YET. BE THE FIRST TO PLAY!";
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
    private enum SchoolType { FEUTech, FEUDiliman, FEUAlabang, Others }

    [Serializable]
    private class LeaderboardMetadata
    {
        public string name;
        public string school;
    }

    private void Start()
    {
        // Set up button listeners
        if (feuTechButton != null)
            feuTechButton.onClick.AddListener(() => SelectSchool(SchoolType.FEUTech));

        if (feuDilimanButton != null)
            feuDilimanButton.onClick.AddListener(() => SelectSchool(SchoolType.FEUDiliman));

        if (feuAlabanButton != null)
            feuAlabanButton.onClick.AddListener(() => SelectSchool(SchoolType.FEUAlabang));

        if (othersButton != null)
            othersButton.onClick.AddListener(() => SelectSchool(SchoolType.Others));

        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(GoToMainMenu);

        if (refreshLeaderboardButton != null)
            refreshLeaderboardButton.onClick.AddListener(() => RefreshLeaderboard());

        // Hide all panels initially (wait for user to click a school button)
        if (feuTechPanel != null) feuTechPanel.SetActive(false);
        if (feuDilimanPanel != null) feuDilimanPanel.SetActive(false);
        if (feuAlabanPanel != null) feuAlabanPanel.SetActive(false);
        if (othersPanel != null) othersPanel.SetActive(false);
    }

    private void SelectSchool(SchoolType school)
    {
        // Hide all panels
        if (feuTechPanel != null) feuTechPanel.SetActive(false);
        if (feuDilimanPanel != null) feuDilimanPanel.SetActive(false);
        if (feuAlabanPanel != null) feuAlabanPanel.SetActive(false);
        if (othersPanel != null) othersPanel.SetActive(false);

        // Show selected panel and load data
        switch (school)
        {
            case SchoolType.FEUTech:
                if (feuTechPanel != null) feuTechPanel.SetActive(true);
                LoadAndDisplaySchoolLeaderboardAsync(school);
                break;
            case SchoolType.FEUDiliman:
                if (feuDilimanPanel != null) feuDilimanPanel.SetActive(true);
                LoadAndDisplaySchoolLeaderboardAsync(school);
                break;
            case SchoolType.FEUAlabang:
                if (feuAlabanPanel != null) feuAlabanPanel.SetActive(true);
                LoadAndDisplaySchoolLeaderboardAsync(school);
                break;
            case SchoolType.Others:
                if (othersPanel != null) othersPanel.SetActive(true);
                LoadAndDisplaySchoolLeaderboardAsync(school);
                break;
        }
    }

    private async void LoadAndDisplaySchoolLeaderboardAsync(SchoolType school)
    {
        string resolvedLeaderboardId = leaderboardId;
        if (string.IsNullOrWhiteSpace(resolvedLeaderboardId) && PlayerManager.Instance != null)
        {
            resolvedLeaderboardId = PlayerManager.Instance.GetLeaderboardId();
        }

        if (string.IsNullOrWhiteSpace(resolvedLeaderboardId))
        {
            Debug.LogError("Leaderboard ID is missing.");
            ShowEmptyState(school);
            return;
        }

        try
        {
            await EnsureSignedInAsync();

            // Get all scores - we need to fetch enough to filter by school
            var allScores = new List<LeaderboardEntry>();
            int offset = 0;
            int limit = 100; // UGS limits per request

            bool hasMore = true;
            while (hasMore)
            {
                var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                    resolvedLeaderboardId,
                    new GetScoresOptions { Offset = offset, Limit = limit, IncludeMetadata = true });

                List<LeaderboardEntry> results = scoresResponse?.Results;
                if (results == null || results.Count == 0)
                {
                    hasMore = false;
                }
                else
                {
                    allScores.AddRange(results);
                    offset += limit;
                    hasMore = results.Count == limit; // If we got fewer than limit, we've reached the end
                }
            }

            // Filter scores by school
            List<LeaderboardEntry> schoolScores = new List<LeaderboardEntry>();
            string targetSchool = GetSchoolName(school);

            foreach (var entry in allScores)
            {
                var meta = ParseMetadata(entry.Metadata);
                if (meta != null)
                {
                    // For OTHERS, include any school that is NOT one of the three FEU schools
                    if (school == SchoolType.Others)
                    {
                        if (meta.school != "FEU-TECH" && meta.school != "FEU-DILIMAN" && meta.school != "FEU-ALABANG")
                        {
                            schoolScores.Add(entry);
                        }
                    }
                    // For FEU schools, match exactly
                    else if (meta.school == targetSchool)
                    {
                        schoolScores.Add(entry);
                    }
                }
            }

            if (schoolScores.Count == 0)
            {
                ShowEmptyState(school);
                return;
            }

            UpdateSchoolLeaderboard(school, schoolScores);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"School leaderboard load failed: {ex.Message}");
            ShowEmptyState(school);
        }
    }

    private string GetSchoolName(SchoolType school)
    {
        switch (school)
        {
            case SchoolType.FEUTech:
                return "FEU-TECH";
            case SchoolType.FEUDiliman:
                return "FEU-DILIMAN";
            case SchoolType.FEUAlabang:
                return "FEU-ALABANG";
            case SchoolType.Others:
                return "OTHERS";
            default:
                return "";
        }
    }

    private void UpdateSchoolLeaderboard(SchoolType school, List<LeaderboardEntry> schoolScores)
    {
        TextMeshProUGUI[] nameTexts = null;
        TextMeshProUGUI[] schoolTexts = null;
        TextMeshProUGUI[] scoreTexts = null;
        TextMeshProUGUI statusText = null;

        switch (school)
        {
            case SchoolType.FEUTech:
                nameTexts = techNameTexts;
                schoolTexts = techSchoolTexts;
                scoreTexts = techScoreTexts;
                statusText = techStatusText;
                break;
            case SchoolType.FEUDiliman:
                nameTexts = dilimanNameTexts;
                schoolTexts = dilimanSchoolTexts;
                scoreTexts = dilimanScoreTexts;
                statusText = dilimanStatusText;
                break;
            case SchoolType.FEUAlabang:
                nameTexts = alabangNameTexts;
                schoolTexts = alabangSchoolTexts;
                scoreTexts = alabangScoreTexts;
                statusText = alabangStatusText;
                break;
            case SchoolType.Others:
                nameTexts = othersNameTexts;
                schoolTexts = othersSchoolTexts;
                scoreTexts = othersScoreTexts;
                statusText = othersStatusText;
                break;
        }

        UpdateStatusText(school, statusText, schoolScores.Count);

        // Take only top 5
        int displayCount = Mathf.Min(MaxEntries, schoolScores.Count);

        for (int i = 0; i < MaxEntries; i++)
        {
            if (i < displayCount)
            {
                var entry = schoolScores[i];
                var meta = ParseMetadata(entry.Metadata);
                string displayName = GetDisplayName(entry, meta);

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

                SetRowActive(i, nameTexts, schoolTexts, scoreTexts, true);
            }
            else
            {
                SetRowActive(i, nameTexts, schoolTexts, scoreTexts, false);
            }
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

    private void SetRowActive(int index, TextMeshProUGUI[] nameTexts, TextMeshProUGUI[] schoolTexts, TextMeshProUGUI[] scoreTexts, bool isActive)
    {
        if (nameTexts[index] != null) nameTexts[index].gameObject.SetActive(isActive);
        if (schoolTexts[index] != null) schoolTexts[index].gameObject.SetActive(isActive);
        if (scoreTexts[index] != null) scoreTexts[index].gameObject.SetActive(isActive);
    }

    private void ShowEmptyState(SchoolType school)
    {
        TextMeshProUGUI[] nameTexts = null;
        TextMeshProUGUI[] schoolTexts = null;
        TextMeshProUGUI[] scoreTexts = null;
        TextMeshProUGUI statusText = null;

        switch (school)
        {
            case SchoolType.FEUTech:
                nameTexts = techNameTexts;
                schoolTexts = techSchoolTexts;
                scoreTexts = techScoreTexts;
                statusText = techStatusText;
                break;
            case SchoolType.FEUDiliman:
                nameTexts = dilimanNameTexts;
                schoolTexts = dilimanSchoolTexts;
                scoreTexts = dilimanScoreTexts;
                statusText = dilimanStatusText;
                break;
            case SchoolType.FEUAlabang:
                nameTexts = alabangNameTexts;
                schoolTexts = alabangSchoolTexts;
                scoreTexts = alabangScoreTexts;
                statusText = alabangStatusText;
                break;
            case SchoolType.Others:
                nameTexts = othersNameTexts;
                schoolTexts = othersSchoolTexts;
                scoreTexts = othersScoreTexts;
                statusText = othersStatusText;
                break;
        }

        for (int i = 0; i < MaxEntries; i++)
        {
            SetRowActive(i, nameTexts, schoolTexts, scoreTexts, false);
        }

        if (statusText != null)
        {
            statusText.text = noScoresMessage;
            statusText.gameObject.SetActive(true);
        }
    }

    private void UpdateStatusText(SchoolType school, TextMeshProUGUI statusText, int scoreCount)
    {
        if (statusText == null)
            return;

        if (scoreCount <= 0)
        {
            statusText.text = noScoresMessage;
        }
        else if (scoreCount >= MaxEntries)
        {
            statusText.text = string.Format(topScoresMessage, MaxEntries);
        }
        else
        {
            int slotsLeft = MaxEntries - scoreCount;
            statusText.text = string.Format(topScoresWithSlotsMessage, scoreCount, slotsLeft);
        }

        statusText.gameObject.SetActive(true);
    }

    public void RefreshLeaderboard()
    {
        SelectSchool(SchoolType.FEUTech);
    }

    private void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
}
