using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip introVideo;
    [SerializeField] private Button skipButton;
    [SerializeField] private float skipDelay = 1.5f;
    [SerializeField] private string quizSceneName = "Quiz Proper";

    private void Start()
    {
        skipButton.gameObject.SetActive(false);
        
        if (videoPlayer != null && introVideo != null)
        {
            videoPlayer.clip = introVideo;
            videoPlayer.Play();
            StartCoroutine(ShowSkipButtonDelayed());
        }
    }

    private IEnumerator ShowSkipButtonDelayed()
    {
        yield return new WaitForSeconds(skipDelay);
        skipButton.gameObject.SetActive(true);
    }

    public void SkipCutscene()
    {
        videoPlayer.Stop();
        LoadQuizScene();
    }

    private void LoadQuizScene()
    {
        SceneManager.LoadScene(quizSceneName);
    }

    private void Update()
    {
        // Also advance if video finishes naturally
        if (videoPlayer.isPlaying == false && videoPlayer.time > 0)
        {
            LoadQuizScene();
        }
    }
}
