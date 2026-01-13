using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LearnModeController : MonoBehaviour
{
    [Header("UI References")]
    public Button[] selectionButtons;   // 17 buttons
    public Image displayImage;          // Image to show in Learn Mode
    public Button nextButton;           // Next button
    public Button prevButton;           // Previous button
    public TMP_Text titleText;              // Title text at the top

    [Header("Content")]
    public Sprite[] learnSprites;       // Array of 17 sprites
    public string[] topicTitles;        // Array of 17 topic titles

    private int currentIndex = 0;

    void Start()
    {
        // Assign listeners for selection buttons
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            int index = i; // local copy for closure
            selectionButtons[i].onClick.AddListener(() => OnSelect(index));
        }

        // Assign listeners for navigation
        nextButton.onClick.AddListener(OnNext);
        prevButton.onClick.AddListener(OnPrev);

        // Hide image and title at start
        displayImage.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
    }

    void OnSelect(int index)
    {
        currentIndex = index;
        ShowContent();
    }

    void OnNext()
    {
        currentIndex++;
        if (currentIndex >= learnSprites.Length)
            currentIndex = 0; // wrap around
        ShowContent();
    }

    void OnPrev()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = learnSprites.Length - 1; // wrap around
        ShowContent();
    }

    void ShowContent()
    {
        displayImage.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);

        displayImage.sprite = learnSprites[currentIndex];
        titleText.text = topicTitles[currentIndex];
    }
}
