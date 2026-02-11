using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LearnModeController : MonoBehaviour
{
    [Header("UI References")]
    public Button[] selectionButtons;   // 17 buttons for Learn Mode
    public Button[] mdgButtons;         // Buttons for MDG selections
    public Button nextButton;           // Next button
    public Button prevButton;           // Previous button
    public TMP_Text titleText;          // Title text at the top
    public TMP_Text contentText;        // Text content for each topic
    public Image iconImage;             // Icon image for each topic

    [Header("Learn Mode Content")]
    public string[] topicTitles;        // Array of Learn Mode titles
    public string[] topicContents;      // Array of Learn Mode descriptions
    public Sprite[] topicIcons;         // Array of Learn Mode icons

    [Header("MDG Content")]
    public string[] mdgTitles;          // Array of MDG titles
    public string[] mdgContents;        // Array of MDG descriptions
    public Sprite[] mdgIcons;           // Array of MDG icons

    private int currentIndex = 0;
    private bool isMDGMode = false;     // Flag to switch between Learn Mode and MDG Mode

    void Start()
    {
        // Assign listeners for Learn Mode buttons
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            int index = i;
            selectionButtons[i].onClick.AddListener(() => OnSelect(index, false));
        }

        // Assign listeners for MDG buttons
        for (int i = 0; i < mdgButtons.Length; i++)
        {
            int index = i;
            mdgButtons[i].onClick.AddListener(() => OnSelect(index, true));
        }

        // Assign listeners for navigation
        nextButton.onClick.AddListener(OnNext);
        prevButton.onClick.AddListener(OnPrev);

        // Hide title, content, and icon at start
        titleText.gameObject.SetActive(false);
        contentText.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
    }

    void OnSelect(int index, bool mdgMode)
    {
        currentIndex = index;
        isMDGMode = mdgMode;
        ShowContent();
    }

    void OnNext()
    {
        currentIndex++;
        int length = isMDGMode ? mdgTitles.Length : topicTitles.Length;
        if (currentIndex >= length)
            currentIndex = 0; // wrap around
        ShowContent();
    }

    void OnPrev()
    {
        currentIndex--;
        int length = isMDGMode ? mdgTitles.Length : topicTitles.Length;
        if (currentIndex < 0)
            currentIndex = length - 1; // wrap around
        ShowContent();
    }

    void ShowContent()
    {
        titleText.gameObject.SetActive(true);
        contentText.gameObject.SetActive(true);
        iconImage.gameObject.SetActive(true);

        if (isMDGMode)
        {
            if (currentIndex < mdgTitles.Length)
                titleText.text = mdgTitles[currentIndex];
            if (currentIndex < mdgContents.Length)
                contentText.text = mdgContents[currentIndex];
            if (mdgIcons != null && currentIndex < mdgIcons.Length)
                iconImage.sprite = mdgIcons[currentIndex];
        }
        else
        {
            if (currentIndex < topicTitles.Length)
                titleText.text = topicTitles[currentIndex];
            if (currentIndex < topicContents.Length)
                contentText.text = topicContents[currentIndex];
            if (topicIcons != null && currentIndex < topicIcons.Length)
                iconImage.sprite = topicIcons[currentIndex];
        }
    }
}
