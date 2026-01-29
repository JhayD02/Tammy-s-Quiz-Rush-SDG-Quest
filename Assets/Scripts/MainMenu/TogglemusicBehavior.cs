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

    private void Start()
    {
        // Initialize toggles
        if (musicToggle != null)
        {
            musicToggle.isOn = !musicSource.mute;
            musicToggle.onValueChanged.AddListener(ToggleMusic);
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = !AreSfxMuted();
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

    private void ToggleMusic(bool isOn)
    {
        if (musicSource != null)
            musicSource.mute = !isOn;
    }

    private void ToggleSFX(bool isOn)
    {
        foreach (AudioSource sfx in sfxSources)
        {
            if (sfx != null)
                sfx.mute = !isOn;
        }

        // Also mute/unmute button click sounds
        if (uiClickSource != null)
            uiClickSource.mute = !isOn;
    }

    private bool AreSfxMuted()
    {
        foreach (AudioSource sfx in sfxSources)
        {
            if (sfx != null && !sfx.mute)
                return false;
        }
        return true;
    }

    private void PlayButtonClick()
    {
        if (uiClickSource != null && buttonClickClip != null && !uiClickSource.mute)
        {
            uiClickSource.PlayOneShot(buttonClickClip);
        }
    }
}
