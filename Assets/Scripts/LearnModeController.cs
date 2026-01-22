using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LearnModeController : MonoBehaviour
{
    [Header("UI References")]
    public Button[] selectionButtons;   // 17 buttons
    public Button nextButton;           // Next button
    public Button prevButton;           // Previous button
    public TMP_Text titleText;          // Title text at the top
    public TMP_Text contentText;        // Text content for each topic
    public Image iconImage;             // Icon image for each topic

    [Header("Content")]
    public string[] topicTitles;        // Array of 17 topic titles
    public string[] topicContents;      // Array of 17 topic descriptions/content
    public Sprite[] topicIcons;         // Array of 17 topic icons

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

        // Hide title, content, and icon at start
        titleText.gameObject.SetActive(false);
        contentText.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
    }

    void OnSelect(int index)
    {
        currentIndex = index;
        ShowContent();
    }

    void OnNext()
    {
        currentIndex++;
        if (currentIndex >= topicTitles.Length)
            currentIndex = 0; // wrap around
        ShowContent();
    }

    void OnPrev()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = topicTitles.Length - 1; // wrap around
        ShowContent();
    }

    void ShowContent()
    {
        titleText.gameObject.SetActive(true);
        contentText.gameObject.SetActive(true);
        iconImage.gameObject.SetActive(true);

        // Show title
        if (currentIndex < topicTitles.Length)
            titleText.text = topicTitles[currentIndex];

        // Show content/description
        if (currentIndex < topicContents.Length)
            contentText.text = topicContents[currentIndex];

        // Show icon
        if (topicIcons != null && currentIndex < topicIcons.Length)
            iconImage.sprite = topicIcons[currentIndex];
    }
}
