using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoLoadingScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string sceneToLoad;
    public float delayAfterVideo = 2f; // ⏱ Delay in seconds after video ends

    void Start()
    {
        StartCoroutine(PlayVideoAndLoadScene());
    }

    IEnumerator PlayVideoAndLoadScene()
    {
        // Start playing video
        videoPlayer.Play();

        // Begin loading scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // Wait until video finishes
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // Optional delay after video ends
        yield return new WaitForSeconds(delayAfterVideo);

        // Activate scene
        asyncLoad.allowSceneActivation = true;
    }
}
