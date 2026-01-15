// LEADERBOARD DEBUG HELPER
// This script adds helpful buttons in the Unity Editor for testing the leaderboard
// To use: Attach to a GameObject and use the buttons in the Inspector

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LeaderboardDebugHelper : MonoBehaviour
{
    [Header("=== DEBUG TOOLS ===")]
    [Tooltip("This helps you test the leaderboard system")]
    public bool showInstructions = true;

    [Header("=== Test Data Generator ===")]
    [SerializeField] private string[] testFirstNames = { "John", "Maria", "Alex", "Sarah", "Mike" };
    [SerializeField] private string[] testLastNames = { "Doe", "Garcia", "Smith", "Johnson", "Brown" };
    [SerializeField] private string[] testSchools = { "TECH", "ALABANG", "DILIMAN", "UP-CEBU" };

    // === PUBLIC METHODS (Can be called from Inspector buttons) ===
    
    public void GenerateTestRecord()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found! Make sure it's in the scene.");
            return;
        }

        // Generate random test data
        string firstName = testFirstNames[Random.Range(0, testFirstNames.Length)];
        string lastName = testLastNames[Random.Range(0, testLastNames.Length)];
        string school = testSchools[Random.Range(0, testSchools.Length)];
        int score = Random.Range(500, 3500);

        // Set player info
        PlayerManager.Instance.SetPlayerInfo(firstName, lastName, school);
        PlayerManager.Instance.SetFinalScore(score);

        Debug.Log($"✓ Generated test record: {firstName} {lastName} from {school} - Score: {score}");
    }

    public void GenerateFiveTestRecords()
    {
        for (int i = 0; i < 5; i++)
        {
            GenerateTestRecord();
        }
        Debug.Log("✓ Generated 5 test records!");
    }

    public void ClearAllRecords()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found!");
            return;
        }

        PlayerManager.Instance.ClearAllRecords();
        Debug.Log("✓ All records cleared!");
    }

    public void ViewSavedRecords()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found!");
            return;
        }

        var database = PlayerManager.Instance.LoadDatabase();
        Debug.Log($"=== SAVED RECORDS ({database.allRecords.Count} total) ===");
        
        for (int i = 0; i < database.allRecords.Count; i++)
        {
            var record = database.allRecords[i];
            Debug.Log($"{i + 1}. {record.firstName} {record.lastName} | {record.school} | {record.timestamp} | Score: {record.finalScore}");
        }
    }

    public void ViewRecentFive()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager not found!");
            return;
        }

        var recent = PlayerManager.Instance.GetRecentRecords(5);
        Debug.Log($"=== 5 MOST RECENT RECORDS ===");
        
        for (int i = 0; i < recent.Count; i++)
        {
            var record = recent[i];
            Debug.Log($"{i + 1}. {record.firstName} {record.lastName} | {record.school} | {record.timestamp} | Score: {record.finalScore}");
        }
    }

    public void RefreshLeaderboardDisplay()
    {
        LocalLeaderboardManager leaderboard = FindObjectOfType<LocalLeaderboardManager>();
        if (leaderboard != null)
        {
            leaderboard.RefreshLeaderboard();
            Debug.Log("✓ Leaderboard refreshed!");
        }
        else
        {
            Debug.LogWarning("LocalLeaderboardManager not found in scene.");
        }
    }

    public void OpenSaveFileLocation()
    {
        string path = Application.persistentDataPath;
        Application.OpenURL(path);
        Debug.Log($"Opening save location: {path}");
    }
}

// === CUSTOM EDITOR (Makes buttons appear in Inspector) ===
#if UNITY_EDITOR
[CustomEditor(typeof(LeaderboardDebugHelper))]
public class LeaderboardDebugHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        LeaderboardDebugHelper helper = (LeaderboardDebugHelper)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== QUICK TEST TOOLS ===", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Generate 1 Test Record", GUILayout.Height(30)))
        {
            helper.GenerateTestRecord();
        }

        if (GUILayout.Button("Generate 5 Test Records", GUILayout.Height(30)))
        {
            helper.GenerateFiveTestRecords();
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("=== VIEW DATA ===", EditorStyles.boldLabel);

        if (GUILayout.Button("View All Saved Records", GUILayout.Height(25)))
        {
            helper.ViewSavedRecords();
        }

        if (GUILayout.Button("View Recent 5", GUILayout.Height(25)))
        {
            helper.ViewRecentFive();
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("=== REFRESH & CLEAR ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Leaderboard Display", GUILayout.Height(25)))
        {
            helper.RefreshLeaderboardDisplay();
        }

        EditorGUILayout.Space(3);
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Records", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Clear All Records?", 
                "This will delete all saved player records. Continue?", "Yes, Clear", "Cancel"))
            {
                helper.ClearAllRecords();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("=== FILE SYSTEM ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Save File Location", GUILayout.Height(25)))
        {
            helper.OpenSaveFileLocation();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Use these buttons to test the leaderboard system without playing the full quiz.", MessageType.Info);
    }
}
#endif
