// SCRIPT 4: UI MANAGER - Handles all the UI screens and panels
// This manages:
// - Instruction Panel (the "welcome" screen)
// - User Information Panel (name, school selection)
// - Validation (making sure all fields are filled)
// - Connecting everything together

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("=== USER INFO PANEL ===")]
    [SerializeField] private GameObject userInfoPanel;
    [SerializeField] private TMP_InputField firstNameInput;
    [SerializeField] private TMP_InputField lastNameInput;
    [SerializeField] private Button[] schoolButtons = new Button[4]; // TECH, ALABANG, DILIMAN, OTHERS
    [SerializeField] private GameObject othersAbbreviationContainer;
    [SerializeField] private TMP_InputField abbreviationInput;
    [SerializeField] private Button startQuizButton;

    [Header("=== COLORS ===")]
    [SerializeField] private Color selectedButtonColor = new Color(0.2f, 0.6f, 1f); // Blue
    [SerializeField] private Color normalButtonColor = Color.white;

    [Header("=== REFERENCES ===")]
    [SerializeField] private QuizProper quizProper;
    [SerializeField] private GameObject quizPanel; // The panel containing the quiz UI

    // Private tracking variables
    private int selectedSchoolIndex = -1; // -1 means no school selected
    private string[] schoolNames = { "TECH", "ALABANG", "DILIMAN", "OTHERS" };

    private void Start()
    {
        // Set up name inputs
        firstNameInput.onValueChanged.AddListener(_ => ValidateForm());
        lastNameInput.onValueChanged.AddListener(_ => ValidateForm());
        abbreviationInput.onValueChanged.AddListener(_ => ValidateForm());

        // Set up school buttons
        for (int i = 0; i < schoolButtons.Length; i++)
        {
            int schoolIndex = i; // Local copy for the closure
            schoolButtons[i].onClick.AddListener(() => SelectSchool(schoolIndex));
        }

        // Set up start quiz button
        startQuizButton.onClick.AddListener(OnStartQuizPressed);

        // Hide panels at start (QuizProper handles instruction panels now)
        othersAbbreviationContainer.SetActive(false);
        startQuizButton.gameObject.SetActive(false); // Hide until all fields valid

        ValidateForm(); // Run once to update button state
    }

    // === USER INFO PANEL ===
    public void ShowUserInfoPanel()
    {
        userInfoPanel.SetActive(true);
        ValidateForm();
    }

    // === SCHOOL SELECTION ===
    private void SelectSchool(int schoolIndex)
    {
        selectedSchoolIndex = schoolIndex;

        // Update button colors - show which one is selected
        for (int i = 0; i < schoolButtons.Length; i++)
        {
            ColorBlock colors = schoolButtons[i].colors;
            colors.normalColor = (i == schoolIndex) ? selectedButtonColor : normalButtonColor;
            colors.selectedColor = colors.normalColor;
            schoolButtons[i].colors = colors;
        }

        // Show/hide the "OTHERS" abbreviation input
        bool isOthersSelected = (schoolIndex == 3);
        othersAbbreviationContainer.SetActive(isOthersSelected);

        if (isOthersSelected)
        {
            abbreviationInput.text = ""; // Clear it when first opened
        }

        ValidateForm();
    }

    // === FORM VALIDATION ===
    // This runs every time a field changes
    // It checks if all required fields are filled
    private void ValidateForm()
    {
        bool firstNameFilled = !string.IsNullOrWhiteSpace(firstNameInput.text);
        bool lastNameFilled = !string.IsNullOrWhiteSpace(lastNameInput.text);
        bool schoolSelected = selectedSchoolIndex >= 0;
        bool abbreviationValid = true;

        // If "OTHERS" is selected, check if abbreviation is filled
        if (schoolSelected && selectedSchoolIndex == 3)
        {
            abbreviationValid = !string.IsNullOrWhiteSpace(abbreviationInput.text);
        }

        // Show the START QUIZ button only if everything is valid
        bool canStartQuiz = firstNameFilled && lastNameFilled && schoolSelected && abbreviationValid;
        startQuizButton.gameObject.SetActive(canStartQuiz);
    }

    // === START QUIZ ===
    private void OnStartQuizPressed()
    {
        // Get the player's inputs
        string firstName = firstNameInput.text.Trim();
        string lastName = lastNameInput.text.Trim();
        string school = schoolNames[selectedSchoolIndex];

        // If they selected "OTHERS", use their custom abbreviation
        if (selectedSchoolIndex == 3)
        {
            school = abbreviationInput.text.Trim().ToUpper();
        }

        // Save player info to PlayerManager (which will save it when quiz ends)
        PlayerManager.Instance.SetPlayerInfo(firstName, lastName, school);

        // Hide the user info panel
        userInfoPanel.SetActive(false);

        // Show the quiz panel
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
            Debug.Log("Quiz panel is now active");
        }
        else
        {
            Debug.LogError("ERROR: quizPanel is not assigned in the UIManager Inspector!");
            return;
        }

        // Start the quiz! (This will show quiz UI and load first question)
        quizProper.StartQuiz();
    }
}
