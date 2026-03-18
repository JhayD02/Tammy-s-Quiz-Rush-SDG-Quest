using UnityEngine;
using UnityEngine.UI;

public class AudioToggleManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;   // Background music
    public AudioSource[] sfxSources;  // All SFX sources
    public AudioSource uiClickSource; // Dedicated AudioSource for UI button clicks

    [Header("UI Toggles")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("UI Buttons")]
    public Button[] buttons;          // Assign all buttons that should play click sounds
    public AudioClip buttonClickClip; // Assign your click sound effect

    private bool sfxEnabled = true;

    private void Awake()
    {
        // Load saved preferences when a new manager is created
        bool musicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("SfxEnabled", 1) == 1;

        ToggleMusic(musicOn);
        ToggleSFX(sfxOn);
    }

    private void Start()
    {
        // Initialize toggles if present in this scene
        if (musicToggle != null)
        {
            musicToggle.isOn = !musicSource.mute;
            musicToggle.onValueChanged.AddListener(ToggleMusic);
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = sfxEnabled;
            sfxToggle.onValueChanged.AddListener(ToggleSFX);
        }

        // Hook up button click sounds
        if (buttons != null && uiClickSource != null && buttonClickClip != null)
        {
            foreach (Button btn in buttons)
            {
                btn.onClick.AddListener(() => PlayButtonClick());
            }
        }
    }

    public void ToggleMusic(bool isOn)
    {
        if (musicSource != null)
            musicSource.mute = !isOn;

        PlayerPrefs.SetInt("MusicEnabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX(bool isOn)
    {
        sfxEnabled = isOn;

        foreach (AudioSource sfx in sfxSources)
        {
            if (sfx != null)
                sfx.mute = !isOn;
        }

        if (uiClickSource != null)
            uiClickSource.mute = !isOn;

        PlayerPrefs.SetInt("SfxEnabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void PlayButtonClick()
    {
        if (sfxEnabled && uiClickSource != null && buttonClickClip != null)
        {
            uiClickSource.PlayOneShot(buttonClickClip);
        }
    }
}
