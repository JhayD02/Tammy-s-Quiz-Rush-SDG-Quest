using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LearnModeController : MonoBehaviour
{
    [Header("UI References")]
    public Button[] selectionButtons;   // 17 buttons for Learn Mode
    public Button nextButton;           // Next button
    public Button prevButton;           // Previous button
    public TMP_Text titleText;          // Title text at the top
    public TMP_Text contentText;        // Text content for each topic
    public Image iconImage;             // Icon image for each topic

    [Header("Learn Mode Content")]
    public string[] topicTitles;        // Array of Learn Mode titles
    public string[] topicContents;      // Array of Learn Mode descriptions
    public Sprite[] topicIcons;         // Array of Learn Mode icons

    private int currentIndex = 0;

    void Start()
    {
        // Assign listeners for Learn Mode buttons
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            int index = i;
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

        if (currentIndex < topicTitles.Length)
            titleText.text = topicTitles[currentIndex];
        if (currentIndex < topicContents.Length)
            contentText.text = topicContents[currentIndex];
        if (topicIcons != null && currentIndex < topicIcons.Length)
            iconImage.sprite = topicIcons[currentIndex];
    }
}
