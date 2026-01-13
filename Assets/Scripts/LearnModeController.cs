using UnityEngine;
using UnityEngine.UI;

public class LearnModeCon : MonoBehaviour
{
    [Header("UI References")]
    public Button[] selectionButtons;   // 17 buttons
    public Image displayImage;          // Image to show in Learn Mode
    public Button nextButton;           // Next button
    public Button prevButton;           // Previous button

    [Header("Images")]
    public Sprite[] learnSprites;       // Array of 17 sprites

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

        // Hide image at start
        displayImage.gameObject.SetActive(false);
    }

    void OnSelect(int index)
    {
        currentIndex = index;
        ShowImage();
    }

    void OnNext()
    {
        currentIndex++;
        if (currentIndex >= learnSprites.Length)
            currentIndex = 0; // wrap around
        ShowImage();
    }

    void OnPrev()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = learnSprites.Length - 1; // wrap around
        ShowImage();
    }

    void ShowImage()
    {
        displayImage.gameObject.SetActive(true);
        displayImage.sprite = learnSprites[currentIndex];
    }
}
