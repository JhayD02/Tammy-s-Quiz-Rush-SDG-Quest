using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class WarningTextController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text warningText;    // Assign your TMP_Text in Inspector
    [SerializeField] private Image warningImage;      // Assign your warning icon Image in Inspector

    [Header("Settings")]
    [SerializeField] private float displayTime = 3f;  // How long text stays visible
    [SerializeField] private float fadeDuration = 1f; // Fade out duration
    [SerializeField] private string nextSceneName;    // Scene to load after warning
    [SerializeField] private bool internetRequired;   // Toggle: true = required, false = optional

    void Start()
    {
        // Ensure text and image are fully visible at start
        SetAlpha(1f);

        StartCoroutine(ShowWarningThenLoad());
    }

    IEnumerator ShowWarningThenLoad()
    {
        // Check internet availability
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (internetRequired)
            {
                warningText.text = "Please ensure your WiFi / Internet Connection is ON\nNo Internet Connection Available - Required to continue";
            }
            else
            {
                warningText.text = "Please ensure your WiFi / Internet Connection is ON\nNo Internet Connection Available - You may continue offline";
            }
        }
        else
        {
            warningText.text = "Please ensure your WiFi / Internet Connection is ON\nInternet Connection Available";
        }

        // Wait for display time
        yield return new WaitForSeconds(displayTime);

        // Fade out text + image together
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }

    private void SetAlpha(float alpha)
    {
        // Fade text
        Color tc = warningText.color;
        tc.a = alpha;
        warningText.color = tc;

        // Fade image
        if (warningImage != null)
        {
            Color ic = warningImage.color;
            ic.a = alpha;
            warningImage.color = ic;
        }
    }
}
