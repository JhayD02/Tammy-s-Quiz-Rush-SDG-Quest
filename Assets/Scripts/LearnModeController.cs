using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LearnModeController : MonoBehaviour
{
    [Header("UI References")]
    public Button[] selectionButtons;   // 17 buttons
    public Image displayImage;          // Main image to show in Learn Mode
    public Image iconImage;             // Icon image for each topic
    public Button nextButton;           // Next button
    public Button prevButton;           // Previous button
    public TMP_Text titleText;          // Title text at the top

    [Header("Content")]
    public Sprite[] learnSprites;       // Array of 17 main sprites
    public Sprite[] topicIcons;         // Array of 17 icon sprites
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

        // Hide image, icon, and title at start
        displayImage.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
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
        iconImage.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);

        // Show main image
        displayImage.sprite = learnSprites[currentIndex];

        // Show icon
        if (topicIcons != null && currentIndex < topicIcons.Length)
        {
            iconImage.sprite = topicIcons[currentIndex];
        }

        // Show title
        titleText.text = topicTitles[currentIndex];
    }
}
    