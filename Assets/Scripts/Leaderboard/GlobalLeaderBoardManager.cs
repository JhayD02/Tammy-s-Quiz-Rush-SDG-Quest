using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.UI;
using TMPro;

public class GlobalLeaderBoardManager : MonoBehaviour
{
    int leaderboardID = 32898;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Submits the player's score to the LootLocker global leaderboard
    /// </summary>
    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");

        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardID.ToString(), (response) =>
        {
            if (response.success)
            {
                Debug.Log("✅ Successfully uploaded score to global leaderboard!");
                done = true;
            }
            else
            {
                Debug.LogError("❌ Failed to upload score: " + response.statusCode);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
