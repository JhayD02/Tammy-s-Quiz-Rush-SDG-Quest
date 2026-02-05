#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalLeaderboardManager))]
public class LocalLeaderboardManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);

        LocalLeaderboardManager manager = (LocalLeaderboardManager)target;

        if (GUILayout.Button("Refresh Leaderboard"))
        {
            manager.RefreshLeaderboard();
        }

        if (GUILayout.Button("Clear All Records (JSON)"))
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Clear All Records",
                "This will delete all saved leaderboard records. Continue?",
                "Yes, Clear",
                "Cancel");

            if (confirm)
            {
                manager.ClearAllRecords();
            }
        }
    }
}
#endif
