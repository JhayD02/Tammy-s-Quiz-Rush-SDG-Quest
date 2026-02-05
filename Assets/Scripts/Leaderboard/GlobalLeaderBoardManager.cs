using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class GlobalLeaderboardManager : MonoBehaviour
{
    [SerializeField] public List<TextMeshProUGUI> names;
    // [SerializeField] public List<TextMeshProUGUI> schools;
    [SerializeField] public List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "ecccdac85ff1e343d37323a2e47dee31299f6290d21b664156da2476fcba2155";

    public void Start()
    {
        GetLeaderboard();
    }
    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) => {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
            for (int i = 0; i < loopLength; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((msg) => {
            GetLeaderboard();
        }));
    }

}

