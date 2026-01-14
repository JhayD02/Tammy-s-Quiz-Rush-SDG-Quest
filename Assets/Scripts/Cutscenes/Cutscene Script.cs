using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneScript : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip introVideo;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] [Range(0f, 1f)] private float videoVolume = 1f;
    
    [Header("Skip Button Settings")]
    [SerializeField] private Button skipButton;
    [SerializeField] private float skipDelay = 1.5f;
    
    [Header("Scene Settings")]
    [Tooltip("Drag the Quiz Scene asset here from Project window")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset quizSceneAsset;
#endif
    [SerializeField] private string quizSceneName = "Quiz Proper Scene";
    
    private bool canSkip = false;
    private float timer = 0f;

#if UNITY_EDITOR
    /// <summary>
    /// Validates scene assignment in editor
    /// </summary>
    private void OnValidate()
    {
        if (quizSceneAsset != null)
        {
            quizSceneName = quizSceneAsset.name;
        }
    }
#endif

    void Start()
    {
        InitializeCutscene();
    }

    void Update()
    {
        HandleSkipDelay();
    }

    /// <summary>
    /// Initializes the cutscene by setting up video and hiding skip button
    /// </summary>
    private void InitializeCutscene()
    {
        // Hide skip button initially
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
            skipButton.onClick.AddListener(SkipCutscene);
        }

        // Setup video player
        if (videoPlayer != null && introVideo != null)
        {
            videoPlayer.clip = introVideo;
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogError("CutsceneScript: VideoPlayer or IntroVideo is not assigned!");
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        ConfigureVideoAudio();
        source.Play();
    }

    /// <summary>
    /// Configures video audio output to AudioSource
    /// </summary>
    private void ConfigureVideoAudio()
    {
        if (audioSource == null)
        {
            audioSource = videoPlayer.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = videoPlayer.gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Check if video has audio tracks
        if (videoPlayer.audioTrackCount > 0)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.SetTargetAudioSource(0, audioSource);
            
            audioSource.playOnAwake = false;
            audioSource.mute = false;
            audioSource.volume = videoVolume;
            
            Debug.Log($"Audio configured: {videoPlayer.audioTrackCount} tracks found, volume: {videoVolume}");
        }
        else
        {
            Debug.LogError("No audio tracks found in video! Re-encode video with H.264 + AAC audio codec.");
        }
    }

    /// <summary>
    /// Handles the skip delay timer and shows button after delay
    /// </summary>
    private void HandleSkipDelay()
    {
        if (!canSkip)
        {
            timer += Time.deltaTime;
            
            if (timer >= skipDelay)
            {
                canSkip = true;
                ShowSkipButton();
            }
        }
    }

    /// <summary>
    /// Shows the skip button with optional fade-in effect
    /// </summary>
    private void ShowSkipButton()
    {
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
            
            // Optional: Add fade-in animation
            StartCoroutine(FadeInButton());
        }
    }

    /// <summary>
    /// Fades in the skip button smoothly
    /// </summary>
    private IEnumerator FadeInButton()
    {
        CanvasGroup canvasGroup = skipButton.GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = skipButton.gameObject.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0f;
        float fadeSpeed = 2f;
        
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    /// <summary>
    /// Skips the cutscene and loads the quiz scene
    /// </summary>
    private void SkipCutscene()
    {
        if (!canSkip) return;
        
        // Stop video playback
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        
        // Load quiz scene
        LoadQuizScene();
    }

    /// <summary>
    /// Called when video reaches the end
    /// </summary>
    private void OnVideoEnd(VideoPlayer vp)
    {
        LoadQuizScene();
    }

    /// <summary>
    /// Loads the quiz proper scene
    /// </summary>
    private void LoadQuizScene()
    {
        if (!string.IsNullOrEmpty(quizSceneName))
        {
            SceneManager.LoadScene(quizSceneName);
        }
        else
        {
            Debug.LogError("CutsceneScript: Quiz scene name is not set!");
        }
    }

    /// <summary>
    /// Clean up event listeners
    /// </summary>
    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
        
        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(SkipCutscene);
        }
    }
}
