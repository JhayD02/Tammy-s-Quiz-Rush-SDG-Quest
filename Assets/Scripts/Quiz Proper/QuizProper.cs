// SCRIPT 2: QUIZ PROPER - The main game logic
// This script handles everything that happens during the quiz:
// - Timer counting down
// - Showing questions
// - Checking answers
// - Managing lifelines
// - Scoring
// - Shuffle answer buttons

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuizProper : MonoBehaviour
{
    [Header("=== INSPECTOR SETTINGS ===")]
    [SerializeField] private List<QuizQuestion> questionBank = new List<QuizQuestion>();
    [SerializeField] private int totalQuestionsInGame = 30;
    [SerializeField] private float timePerQuestion = 20f;

    [Header("=== UI ELEMENTS ===")]
    [SerializeField] private TextMeshProUGUI questionLabel;
    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private TextMeshProUGUI questionCounterLabel;
    [SerializeField] private Button[] answerButtons = new Button[4]; // Position 1,2,3,4
    [SerializeField] private TextMeshProUGUI[] answerTexts = new TextMeshProUGUI[4]; // Text on each button
    [SerializeField] private Image[] answerImages = new Image[4]; // Images on each button
    [SerializeField] private Button nextQuestionButton;
    [SerializeField] private CanvasGroup nextButtonCanvasGroup; // For fade in effect
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private string homeSceneName = "Home";

    [Header("=== LIFELINE BUTTONS ===")]
    [SerializeField] private Button stopTimeButton;
    [SerializeField] private Button doublePointsButton;
    [SerializeField] private Button reduceChoicesButton;
    [SerializeField] private float lifelineDisabledOpacity = 0.4f;

    [Header("=== LIFELINE GAINED PANEL ===")]
    [SerializeField] private GameObject lifelineGainedPanel;
    [SerializeField] private TextMeshProUGUI lifelineGainedText;
    [SerializeField] private Image lifelineGainedImage;
    [SerializeField] private Button lifelineContinueButton;
    [SerializeField] private float lifelineRouletteDuration = 2f;
    [SerializeField] private float lifelineRouletteInterval = 0.15f;
    [SerializeField] private Sprite stopTimeSprite;
    [SerializeField] private Sprite doublePointsSprite;
    [SerializeField] private Sprite reduceChoicesSprite;

    [Header("=== ANIMATION SETTINGS ===")]
    [SerializeField] private float nextButtonFadeDelay = 1.5f;
    [SerializeField] private float nextButtonFadeDuration = 0.4f;
    [SerializeField] private float scoreIncrementSpeed = 0.02f;

    [Header("=== SCORING MULTIPLIERS (CHANGE THESE) ===")]
    [SerializeField] private int pointsFastAnswer = 120; // 5 seconds or less
    [SerializeField] private int pointsMediumAnswer = 110; // 6-12 seconds
    [SerializeField] private int pointsSlowAnswer = 100; // 13+ seconds
    [SerializeField] private int lifelineBonus = 150; // Per unused lifeline at end

    [Header("=== RESULTS PANEL ===")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private CanvasGroup resultsPanelCanvasGroup;
    [SerializeField] private TextMeshProUGUI resultsMessageText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button resultsNextButton; // Button to go to cutscene
    [SerializeField] private float resultsPanelFadeDelay = 1.0f;
    [SerializeField] private float resultsPanelFadeDuration = 1.0f;
    
    [Header("=== CUTSCENE SETTINGS ===")]
    [SerializeField] private string goodCutsceneSceneName = "GoodCutscene";
    [SerializeField] private string badCutsceneSceneName = "BadCutscene";
    [SerializeField] private int cutsceneScoreThreshold = 2400; // Score needed for good ending
    
    [Header("=== INSTRUCTION PANELS ===")]
    [SerializeField] private GameObject instructionPanel1;
    [SerializeField] private Button instructionPanel1NextButton;
    [SerializeField] private GameObject instructionPanel2;
    [SerializeField] private Button instructionPanel2NextButton;
    [SerializeField] private UIManager uiManager; // Reference to UIManager for user info panel
    
    [Header("=== PERFORMANCE FEEDBACK MESSAGES ===")]
    [TextArea(2, 3)]
    [SerializeField] private string excellentMessage = "Excellent!";
    [TextArea(2, 3)]
    [SerializeField] private string goodMessage = "Good!";
    [TextArea(2, 3)]
    [SerializeField] private string needsImprovementMessage = "Need more improvement";
    
    [Header("=== SCORE THRESHOLDS ===")]
    [SerializeField] private int excellentThreshold = 2500;
    [SerializeField] private int goodThreshold = 1000;

    // Private variables - these change during the game
    private List<QuizQuestion> shuffledQuestions = new List<QuizQuestion>();
    private int currentQuestionIndex = 0;
    private int currentScore = 0;
    private float timeRemaining = 0f;
    private bool isAnswering = false; // Is a question currently being answered?
    private bool quizStarted = false;

    // Lifeline tracking
    private bool hasStopTime = false;
    private bool hasDoublePoints = false;
    private bool hasReduceChoices = false;
    private bool pendingDoublePoints = false; // Tracks if next correct answer should be doubled
    private int correctAnswerStreak = 0; // For streak-based lifeline reward

    // Button position shuffling - remember initial positions for swapping
    private Vector2[] initialButtonPositions = new Vector2[4]; // Store initial anchoredPosition of each button
    private bool positionsInitialized = false;

    // Coroutines
    private Coroutine timerCoroutine;
    private Coroutine fadeInNextButtonCoroutine;
    private Coroutine lifelineRouletteCoroutine;

    // Lifeline panel tracking
    private bool isGuaranteedLifelinePending = false;
    private bool isStreakLifelinePending = false;

    private enum LifelineType { StopTime, DoublePoints, ReduceChoices }

    private void Start()
    {
        // Set up all button listeners
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(buttonIndex));
        }

        nextQuestionButton.onClick.AddListener(OnNextQuestionClicked);
        pauseButton.onClick.AddListener(PauseTheQuiz);
        resumeButton.onClick.AddListener(ResumeTheQuiz);
        restartButton.onClick.AddListener(RestartTheQuiz);
        homeButton.onClick.AddListener(GoHome);

        stopTimeButton.onClick.AddListener(UseStopTime);
        doublePointsButton.onClick.AddListener(UseDoublePoints);
        reduceChoicesButton.onClick.AddListener(UseReduceChoices);
        if (lifelineContinueButton != null)
            lifelineContinueButton.onClick.AddListener(OnLifelineContinueClicked);

        // Set up results panel next button
        if (resultsNextButton != null)
            resultsNextButton.onClick.AddListener(OnResultsNextClicked);

        // Set up instruction panel buttons
        if (instructionPanel1NextButton != null)
        {
            instructionPanel1NextButton.onClick.RemoveAllListeners(); // Clear any existing listeners
            instructionPanel1NextButton.onClick.AddListener(OnInstructionPanel1NextClicked);
            Debug.Log("Instruction Panel 1 Next Button listener added");
        }
        else
        {
            Debug.LogWarning("Instruction Panel 1 Next Button is not assigned!");
        }

        if (instructionPanel2NextButton != null)
        {
            instructionPanel2NextButton.onClick.RemoveAllListeners(); // Clear any existing listeners
            instructionPanel2NextButton.onClick.AddListener(OnInstructionPanel2NextClicked);
            Debug.Log("Instruction Panel 2 Next Button listener added");
        }
        else
        {
            Debug.LogWarning("Instruction Panel 2 Next Button is not assigned!");
        }

        // Store initial button positions for shuffling
        InitializeButtonPositions();

        // Hide UI elements at start
        nextQuestionButton.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        if (lifelineGainedPanel != null)
            lifelineGainedPanel.SetActive(false);

        // Show instruction panel 1 on startup
        ShowInstructionPanel1();

        UpdateLifelineButtons();
    }

    // Store the initial positions of all answer buttons
    private void InitializeButtonPositions()
    {
        if (positionsInitialized) return;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            RectTransform rectTransform = answerButtons[i].GetComponent<RectTransform>();
            initialButtonPositions[i] = rectTransform.anchoredPosition;
        }
        positionsInitialized = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If pause panel is open, resume instead
            if (pausePanel.activeSelf)
            {
                ResumeTheQuiz();
            }
            // If in quiz, pause it
            else if (quizStarted && isAnswering)
            {
                PauseTheQuiz();
            }
        }
    }

    // This is called from UIManager when the player presses "START QUIZ"
    public void StartQuiz()
    {
        // Validation for questionBank content
        if (questionBank == null || questionBank.Count == 0)
        {
            Debug.LogError("ERROR: questionBank is empty or not assigned! Please add questions in the Inspector.");
            return;
        }

        // Show the main quiz UI
        ShowMainQuizUI();

        quizStarted = true;

        // Shuffle all questions and pick the first 30
        shuffledQuestions = questionBank.OrderBy(x => Random.value).Take(totalQuestionsInGame).ToList();
        
        currentQuestionIndex = 0;
        currentScore = 0;
        correctAnswerStreak = 0;
        hasStopTime = false;
        hasDoublePoints = false;
        hasReduceChoices = false;

        UpdateScoreLabel();
        LoadQuestion(0);
    }

    private void LoadQuestion(int index)
    {
        if (index >= shuffledQuestions.Count)
        {
            // Quiz finished! 30 questions done
            FinishQuiz();
            return;
        }

        QuizQuestion currentQuestion = shuffledQuestions[index];

        // Validate the question
        if (currentQuestion == null)
        {
            Debug.LogError($"ERROR: Question at index {index} is null!");
            return;
        }

        if (currentQuestion.answers == null || currentQuestion.answers.Count < 4)
        {
            Debug.LogError($"ERROR: Question {index} has invalid answers array!");
            return;
        }

        if (questionLabel == null)
        {
            Debug.LogError("ERROR: questionLabel is not assigned in the Inspector!");
            return;
        }

        // Display the question
        questionLabel.text = currentQuestion.questionText;

        // Display the question counter
        UpdateQuestionCounter();

        // Display all 4 answer choices (content stays in same button)
        for (int i = 0; i < 4; i++)
        {
            // Validate UI elements exist
            if (answerTexts[i] == null)
            {
                Debug.LogError($"ERROR: answerTexts[{i}] is not assigned in the Inspector!");
                return;
            }
            if (answerButtons[i] == null)
            {
                Debug.LogError($"ERROR: answerButtons[{i}] is not assigned in the Inspector!");
                return;
            }

            AnswerChoice answer = currentQuestion.answers[i];

            // DEBUG: Log what we're about to display
            Debug.Log($"Question {index}, Answer {i}: useImage={answer.useImage}, text='{answer.answerText}'");

            // Show either text or image based on the answer choice
            if (answer.useImage)
            {
                // Only validate answerImages if we're actually using it
                if (answerImages[i] == null)
                {
                    Debug.LogError($"ERROR: answerImages[{i}] is not assigned but useImage is true!");
                    return;
                }

                Debug.Log($"  -> Showing IMAGE for answer {i}");
                answerTexts[i].gameObject.SetActive(false);
                answerImages[i].gameObject.SetActive(true);
                answerImages[i].sprite = answer.answerImage;
            }
            else
            {
                Debug.Log($"  -> Showing TEXT for answer {i}: '{answer.answerText}'");
                answerTexts[i].gameObject.SetActive(true);
                
                // Deactivate image if it exists
                if (answerImages[i] != null)
                {
                    answerImages[i].gameObject.SetActive(false);
                }
                
                answerTexts[i].text = answer.answerText;
                
                // DEBUG: Make sure the text actually updated
                Debug.Log($"  -> answerTexts[{i}].text is now: '{answerTexts[i].text}'");
            }

            // Make button clickable
            answerButtons[i].interactable = true;
        }

        // Shuffle the answer button positions AFTER setting content
        ShuffleAnswerButtonPositions();

        // Hide the next button and reset it
        nextQuestionButton.gameObject.SetActive(false);
        nextButtonCanvasGroup.alpha = 0f;

        // Start the timer
        if (isGuaranteedLifelinePending || isStreakLifelinePending)
        {
            // Don't start answering yet, show lifeline panel first
            if (lifelineRouletteCoroutine != null)
                StopCoroutine(lifelineRouletteCoroutine);
            lifelineRouletteCoroutine = StartCoroutine(ShowLifelineGainedPanelThenContinueQuiz());
        }
        else
        {
            isAnswering = true;
            timeRemaining = timePerQuestion;
            UpdateTimerLabel();

            // Update lifeline button states for the new question
            UpdateLifelineButtons();

            if (timerCoroutine != null)
                StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(TimerCountDown());
        }
    }

    // === ANSWER BUTTON POSITION SHUFFLING ===
    // Buttons are vertically aligned (Button 1, 2, 3, 4 from top to bottom)
    // Button 1 and Button 3 can swap their VISUAL POSITIONS
    // Button 2 and Button 4 can swap their VISUAL POSITIONS
    // The content stays in each button, only positions change
    private void ShuffleAnswerButtonPositions()
    {
        // First, reset all buttons to their initial positions
        for (int i = 0; i < 4; i++)
        {
            RectTransform rectTransform = answerButtons[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = initialButtonPositions[i];
        }

        // Randomly decide if Button 1 and Button 3 swap positions
        bool swapButtons1And3 = Random.value > 0.5f;
        if (swapButtons1And3)
        {
            // Swap the visual positions
            RectTransform button1 = answerButtons[0].GetComponent<RectTransform>();
            RectTransform button3 = answerButtons[2].GetComponent<RectTransform>();
            
            Vector2 temp = button1.anchoredPosition;
            button1.anchoredPosition = button3.anchoredPosition;
            button3.anchoredPosition = temp;
        }

        // Randomly decide if Button 2 and Button 4 swap positions
        bool swapButtons2And4 = Random.value > 0.5f;
        if (swapButtons2And4)
        {
            // Swap the visual positions
            RectTransform button2 = answerButtons[1].GetComponent<RectTransform>();
            RectTransform button4 = answerButtons[3].GetComponent<RectTransform>();
            
            Vector2 temp = button2.anchoredPosition;
            button2.anchoredPosition = button4.anchoredPosition;
            button4.anchoredPosition = temp;
        }
    }

    // === TIMER ===
    private IEnumerator TimerCountDown()
    {
        while (timeRemaining > 0f && isAnswering)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerLabel();
            yield return null;
        }

        // If we get here, time ran out
        if (isAnswering)
        {
            OnTimeExpired();
        }
    }

    private void UpdateTimerLabel()
    {
        timerLabel.text = Mathf.Max(0, Mathf.RoundToInt(timeRemaining)).ToString();
    }

    // === ANSWER HANDLING ===
    private void OnAnswerButtonClicked(int buttonIndex)
    {
        if (!isAnswering)
            return;

        isAnswering = false;

        // Stop the timer
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        QuizQuestion currentQuestion = shuffledQuestions[currentQuestionIndex];
        // buttonIndex directly corresponds to answer index (Button 0 = Answer 0, etc.)
        // The visual position may have been shuffled, but content stays with the button

        // Check if correct
        if (buttonIndex == currentQuestion.correctAnswerIndex)
        {
            HandleCorrectAnswer(currentQuestion);
        }
        else
        {
            HandleWrongAnswer(currentQuestion);
        }
    }

    private void OnTimeExpired()
    {
        isAnswering = false;
        correctAnswerStreak = 0; // Lose streak
        QuizQuestion currentQuestion = shuffledQuestions[currentQuestionIndex];
        ShowFeedback(currentQuestion, "TIMEOUT", currentQuestion.correctAnswerIndex);
    }

    private void HandleCorrectAnswer(QuizQuestion question)
    {
        correctAnswerStreak++;
        
        // Calculate points based on time taken
        float timeTaken = timePerQuestion - timeRemaining;
        int points = 0;

        if (timeTaken <= 5f)
            points = pointsFastAnswer;
        else if (timeTaken <= 12f)
            points = pointsMediumAnswer;
        else
            points = pointsSlowAnswer;

        // Apply double points if pending (for this question only)
        if (pendingDoublePoints)
        {
            points *= 2;
            pendingDoublePoints = false; // Consume the pending double
        }

        AddScore(points);

        // Check for streak lifeline at ANY question
        bool isStreakLifeline = ShouldTriggerStreakLifeline();
        // Check for guaranteed lifeline at questions 15 and 25
        bool isGuaranteedLifeline = IsGuaranteedLifelineQuestion();
        // Check if can award any lifeline
        bool canAwardLifeline = !AllLifelinesOwned();
        // Check if there's a next question to show lifeline on
        bool hasNextQuestion = currentQuestionIndex + 1 < totalQuestionsInGame;

        if ((isStreakLifeline || isGuaranteedLifeline) && canAwardLifeline && hasNextQuestion)
        {
            // For streak lifeline, defer to next question
            if (isStreakLifeline)
            {
                isStreakLifelinePending = true;
                correctAnswerStreak = 0; // Reset streak because reward will be granted
                ShowFeedback(question, "CORRECT", question.correctAnswerIndex, true); // Show next button
            }
            // For guaranteed lifeline (already handled in HandleWrongAnswer)
            // This case handles correct answer on Q15/Q25
            else if (isGuaranteedLifeline)
            {
                isGuaranteedLifelinePending = true;
                ShowFeedback(question, "CORRECT", question.correctAnswerIndex, true); // Show next button
            }
        }
        else
        {
            ShowFeedback(question, "CORRECT", question.correctAnswerIndex);
        }
    }

    private void HandleWrongAnswer(QuizQuestion question)
    {
        correctAnswerStreak = 0; // Lose streak

        // Check if this is a guaranteed lifeline question (15 or 25)
        bool isGuaranteedLifeline = IsGuaranteedLifelineQuestion();
        bool canAwardLifeline = !AllLifelinesOwned();

        if (isGuaranteedLifeline && canAwardLifeline)
        {
            isGuaranteedLifelinePending = true;
            ShowFeedback(question, "WRONG", question.correctAnswerIndex, true);
        }
        else
        {
            ShowFeedback(question, "WRONG", question.correctAnswerIndex);
        }
    }

    private void ShowFeedback(QuizQuestion question, string feedbackType, int correctAnswerIndex, bool autoShowNextButton = true)
    {
        string feedbackMessage = "";

        // Choose the appropriate feedback from the question
        if (feedbackType == "CORRECT")
        {
            feedbackMessage = question.correctFeedback;
        }
        else if (feedbackType == "WRONG")
        {
            feedbackMessage = question.wrongFeedback;
        }
        else if (feedbackType == "TIMEOUT")
        {
            feedbackMessage = question.timeUpFeedback;
        }
        
        questionLabel.text = feedbackMessage;

        // Show the next button after a delay (unless a lifeline panel is showing)
        if (autoShowNextButton)
        {
            if (fadeInNextButtonCoroutine != null)
                StopCoroutine(fadeInNextButtonCoroutine);
            fadeInNextButtonCoroutine = StartCoroutine(FadeInNextButton());
        }
    }

    private IEnumerator FadeInNextButton()
    {
        yield return new WaitForSeconds(nextButtonFadeDelay);
        
        nextQuestionButton.gameObject.SetActive(true);
        nextButtonCanvasGroup.alpha = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < nextButtonFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            nextButtonCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / nextButtonFadeDuration);
            yield return null;
        }

        nextButtonCanvasGroup.alpha = 1f;
    }

    // === SCORE ===
    private void AddScore(int points)
    {
        int startScore = currentScore;
        currentScore += points;
        StartCoroutine(AnimateScoreIncrease(startScore, currentScore));
    }

    private IEnumerator AnimateScoreIncrease(int from, int to)
    {
        for (int i = from; i <= to; i++)
        {
            scoreLabel.text = i.ToString();
            yield return new WaitForSeconds(scoreIncrementSpeed);
        }
    }

    private void UpdateScoreLabel()
    {
        scoreLabel.text = currentScore.ToString();
    }

    private void UpdateQuestionCounter()
    {
        if (questionCounterLabel != null)
        {
            int questionNumber = currentQuestionIndex + 1; // Convert 0-based to 1-based
            questionCounterLabel.text = $"Question {questionNumber}/{totalQuestionsInGame}";
        }
    }

    // === LIFELINES ===
    private void GiveRandomLifeline()
    {
        List<int> availableLifelines = new List<int>();
        if (!hasStopTime) availableLifelines.Add(0);
        if (!hasDoublePoints) availableLifelines.Add(1);
        if (!hasReduceChoices) availableLifelines.Add(2);

        if (availableLifelines.Count == 0)
            return;

        int randomChoice = availableLifelines[Random.Range(0, availableLifelines.Count)];
        
        if (randomChoice == 0) hasStopTime = true;
        else if (randomChoice == 1) hasDoublePoints = true;
        else if (randomChoice == 2) hasReduceChoices = true;

        UpdateLifelineButtons();
    }

    private void UseStopTime()
    {
        if (!hasStopTime || !isAnswering)
            return;

        hasStopTime = false;
        
        // Stop the timer from counting down
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        UpdateLifelineButtons();
    }

    private void UseDoublePoints()
    {
        if (!hasDoublePoints || !isAnswering)
            return;

        hasDoublePoints = false; // Consume the lifeline
        pendingDoublePoints = true; // Mark that next correct answer should be doubled
        // This will be applied when the answer is submitted
        UpdateLifelineButtons();
    }

    private void UseReduceChoices()
    {
        if (!hasReduceChoices || !isAnswering)
            return;

        hasReduceChoices = false;

        QuizQuestion currentQuestion = shuffledQuestions[currentQuestionIndex];
        
        // Find wrong answers and disable two (50/50 choice)
        List<int> wrongAnswers = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (i != currentQuestion.correctAnswerIndex)
            {
                wrongAnswers.Add(i);
            }
        }

        // Disable 2 random wrong answers
        if (wrongAnswers.Count >= 2)
        {
            // Shuffle the wrong answers
            for (int i = wrongAnswers.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                int temp = wrongAnswers[i];
                wrongAnswers[i] = wrongAnswers[randomIndex];
                wrongAnswers[randomIndex] = temp;
            }

            // Disable the first 2 wrong answers
            answerButtons[wrongAnswers[0]].interactable = false;
            answerButtons[wrongAnswers[1]].interactable = false;
        }

        UpdateLifelineButtons();
    }

    private void UpdateLifelineButtons()
    {
        SetLifelineButtonState(stopTimeButton, hasStopTime && isAnswering);
        SetLifelineButtonState(doublePointsButton, hasDoublePoints && isAnswering);
        SetLifelineButtonState(reduceChoicesButton, hasReduceChoices && isAnswering);
    }

    private void SetLifelineButtonState(Button button, bool enabled)
    {
        if (button == null) return;

        button.interactable = enabled;
        CanvasGroup cg = button.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = button.gameObject.AddComponent<CanvasGroup>();
        }

        cg.alpha = enabled ? 1f : lifelineDisabledOpacity;
    }

    private bool ShouldTriggerLifelinePanel()
    {
        int questionNumber = currentQuestionIndex + 1; // convert 0-based to human friendly
        bool isMilestone = questionNumber == 5 || questionNumber == 15 || questionNumber == 25;
        bool hasStreak = correctAnswerStreak == 5;
        bool allOwned = hasStopTime && hasDoublePoints && hasReduceChoices;

        return isMilestone && hasStreak && !allOwned;
    }

    private bool ShouldTriggerStreakLifeline()
    {
        // Trigger lifeline panel anytime player gets 5-correct streak, at ANY question
        return correctAnswerStreak == 5;
    }

    private bool IsGuaranteedLifelineQuestion()
    {
        int questionNumber = currentQuestionIndex + 1;
        return questionNumber == 15 || questionNumber == 25;
    }

    private bool AllLifelinesOwned()
    {
        return hasStopTime && hasDoublePoints && hasReduceChoices;
    }

    private List<LifelineType> GetAvailableLifelines()
    {
        List<LifelineType> available = new List<LifelineType>();
        if (!hasStopTime) available.Add(LifelineType.StopTime);
        if (!hasDoublePoints) available.Add(LifelineType.DoublePoints);
        if (!hasReduceChoices) available.Add(LifelineType.ReduceChoices);
        return available;
    }

    private void AwardLifeline(LifelineType type)
    {
        if (type == LifelineType.StopTime) hasStopTime = true;
        if (type == LifelineType.DoublePoints) hasDoublePoints = true;
        if (type == LifelineType.ReduceChoices) hasReduceChoices = true;
        UpdateLifelineButtons();
    }

    private string GetLifelineDisplayName(LifelineType type)
    {
        if (type == LifelineType.StopTime) return "Stop Time";
        if (type == LifelineType.DoublePoints) return "Double Points";
        return "Guided Questions"; // Reduce choices
    }

    private Sprite GetLifelineSprite(LifelineType type)
    {
        if (type == LifelineType.StopTime) return stopTimeSprite;
        if (type == LifelineType.DoublePoints) return doublePointsSprite;
        return reduceChoicesSprite;
    }

    private IEnumerator ShowLifelineGainedPanel()
    {
        if (lifelineGainedPanel == null)
            yield break;

        List<LifelineType> available = GetAvailableLifelines();
        if (available.Count == 0)
            yield break;

        lifelineGainedPanel.SetActive(true);
        if (lifelineContinueButton != null)
            lifelineContinueButton.gameObject.SetActive(false);

        float elapsed = 0f;
        LifelineType currentShown = available[0];

        // Fast roulette animation cycling through available lifelines
        while (elapsed < lifelineRouletteDuration)
        {
            currentShown = available[Random.Range(0, available.Count)];
            UpdateLifelinePanelVisuals(currentShown, true);
            yield return new WaitForSeconds(lifelineRouletteInterval);
            elapsed += lifelineRouletteInterval;
        }

        // Final award selection
        LifelineType awarded = available.Count == 1 ? available[0] : available[Random.Range(0, available.Count)];
        AwardLifeline(awarded);
        UpdateLifelinePanelVisuals(awarded, false);

        if (lifelineContinueButton != null)
            lifelineContinueButton.gameObject.SetActive(true);
    }

    private IEnumerator ShowLifelineGainedPanelThenContinueQuiz()
    {
        if (lifelineGainedPanel == null)
            yield break;

        List<LifelineType> available = GetAvailableLifelines();
        if (available.Count == 0)
            yield break;

        // Reset both pending flags at start (they'll stay reset through the animation)
        isGuaranteedLifelinePending = false;
        isStreakLifelinePending = false;

        // Hide the next button so it doesn't interfere
        if (nextQuestionButton != null)
            nextQuestionButton.gameObject.SetActive(false);

        lifelineGainedPanel.SetActive(true);
        if (lifelineContinueButton != null)
            lifelineContinueButton.gameObject.SetActive(false);

        float elapsed = 0f;
        LifelineType currentShown = available[0];

        // Fast roulette animation cycling through available lifelines
        while (elapsed < lifelineRouletteDuration)
        {
            currentShown = available[Random.Range(0, available.Count)];
            UpdateLifelinePanelVisuals(currentShown, true);
            yield return new WaitForSeconds(lifelineRouletteInterval);
            elapsed += lifelineRouletteInterval;
        }

        // Final award selection
        LifelineType awarded = available.Count == 1 ? available[0] : available[Random.Range(0, available.Count)];
        AwardLifeline(awarded);
        UpdateLifelinePanelVisuals(awarded, false);

        if (lifelineContinueButton != null)
            lifelineContinueButton.gameObject.SetActive(true);
    }

    private void UpdateLifelinePanelVisuals(LifelineType type, bool showRolling)
    {
        if (lifelineGainedText != null)
        {
            string name = GetLifelineDisplayName(type);
            lifelineGainedText.text = showRolling ? "Congratulations! You gained ..." : $"Congratulations! You gained {name}";
        }

        if (lifelineGainedImage != null)
        {
            lifelineGainedImage.sprite = GetLifelineSprite(type);
            lifelineGainedImage.enabled = lifelineGainedImage.sprite != null;
        }
    }

    private void OnLifelineContinueClicked()
    {
        if (lifelineGainedPanel != null)
            lifelineGainedPanel.SetActive(false);

        // Lifeline panel was shown before timer started, so always start the quiz now
        // Flags are already reset in ShowLifelineGainedPanelThenContinueQuiz
        isAnswering = true;
        timeRemaining = timePerQuestion;
        UpdateTimerLabel();
        UpdateLifelineButtons();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCountDown());
    }

    // === NEXT QUESTION ===
    private void OnNextQuestionClicked()
    {
        currentQuestionIndex++;
        LoadQuestion(currentQuestionIndex);
    }

    // === PAUSE/RESUME ===
    private void PauseTheQuiz()
    {
        if (!isAnswering)
            return;

        isAnswering = false;
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Freeze everything
    }

    private void ResumeTheQuiz()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Unfreeze

        isAnswering = true;
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCountDown());
    }

    private void RestartTheQuiz()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void GoHome()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(homeSceneName);
    }

    // === FINISH QUIZ ===
    private void FinishQuiz()
    {
        isAnswering = false;

        // Add bonuses for unused lifelines
        int lifelineBonusPoints = 0;
        if (hasStopTime) lifelineBonusPoints += lifelineBonus;
        if (hasDoublePoints) lifelineBonusPoints += lifelineBonus;
        if (hasReduceChoices) lifelineBonusPoints += lifelineBonus;

        AddScore(lifelineBonusPoints);

        // Save the player's result
        PlayerManager.Instance.SetFinalScore(currentScore);

        // Show finish panel or do something
        Debug.Log("Quiz finished! Final score: " + currentScore);

        // Show results panel with fade in effect
        StartCoroutine(ShowResultsPanel());
    }

    // Coroutine to fade in the results panel
    private IEnumerator ShowResultsPanel()
    {
        // Wait a moment before showing results
        yield return new WaitForSeconds(resultsPanelFadeDelay);

        // Make sure results panel exists
        if (resultsPanel == null)
        {
            Debug.LogWarning("Results panel not assigned! Please assign it in the Inspector.");
            yield break;
        }

        // Determine performance message based on score
        string performanceMessage;
        if (currentScore >= excellentThreshold)
        {
            performanceMessage = excellentMessage;
        }
        else if (currentScore >= goodThreshold)
        {
            performanceMessage = goodMessage;
        }
        else
        {
            performanceMessage = needsImprovementMessage;
        }

        // Set the text
        if (resultsMessageText != null)
            resultsMessageText.text = performanceMessage;
        
        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {currentScore}";

        // Show the panel
        resultsPanel.SetActive(true);

        // Fade in effect
        if (resultsPanelCanvasGroup != null)
        {
            resultsPanelCanvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < resultsPanelFadeDuration)
            {
                elapsed += Time.deltaTime;
                resultsPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / resultsPanelFadeDuration);
                yield return null;
            }

            resultsPanelCanvasGroup.alpha = 1f;
        }
    }

    // === INSTRUCTION PANELS ===
    private void HideMainQuizUI()
    {
        // Hide all quiz UI elements
        questionLabel.gameObject.SetActive(false);
        timerLabel.gameObject.SetActive(false);
        scoreLabel.gameObject.SetActive(false);
        questionCounterLabel.gameObject.SetActive(false);
        nextQuestionButton.gameObject.SetActive(false);
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(false);
        }

        // Hide lifeline buttons
        if (stopTimeButton != null)
            stopTimeButton.gameObject.SetActive(false);
        if (doublePointsButton != null)
            doublePointsButton.gameObject.SetActive(false);
        if (reduceChoicesButton != null)
            reduceChoicesButton.gameObject.SetActive(false);

        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false);
    }

    private void ShowMainQuizUI()
    {
        // Show all quiz UI elements
        questionLabel.gameObject.SetActive(true);
        timerLabel.gameObject.SetActive(true);
        scoreLabel.gameObject.SetActive(true);
        if (questionCounterLabel != null)
            questionCounterLabel.gameObject.SetActive(true);
        
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(true);
        }

        // Show lifeline buttons
        if (stopTimeButton != null)
            stopTimeButton.gameObject.SetActive(true);
        if (doublePointsButton != null)
            doublePointsButton.gameObject.SetActive(true);
        if (reduceChoicesButton != null)
            reduceChoicesButton.gameObject.SetActive(true);

        if (pauseButton != null)
            pauseButton.gameObject.SetActive(true);
    }

    private void ShowInstructionPanel1()
    {
        // Hide quiz UI
        HideMainQuizUI();

        // Hide all other panels
        if (instructionPanel2 != null)
            instructionPanel2.SetActive(false);
        pausePanel.SetActive(false);
        if (lifelineGainedPanel != null)
            lifelineGainedPanel.SetActive(false);
        resultsPanel.SetActive(false);

        // Show instruction panel 1
        if (instructionPanel1 != null)
        {
            instructionPanel1.SetActive(true);
            Debug.Log("Instruction Panel 1 is now active");
        }
        else
        {
            Debug.LogWarning("Instruction Panel 1 is not assigned in the Inspector!");
        }
    }

    private void OnInstructionPanel1NextClicked()
    {
        Debug.Log("=== Instruction Panel 1 Next Button clicked! ===");
        
        // Hide instruction panel 1
        if (instructionPanel1 != null)
        {
            instructionPanel1.SetActive(false);
            Debug.Log("Instruction Panel 1 hidden");
        }

        // IMPORTANT: Do NOT start quiz yet
        // Show instruction panel 2 ONLY
        ShowInstructionPanel2();
        
        Debug.Log("OnInstructionPanel1NextClicked completed");
    }

    private void ShowInstructionPanel2()
    {
        Debug.Log("Showing Instruction Panel 2");
        
        // Hide quiz UI
        HideMainQuizUI();

        // Hide all other panels
        if (instructionPanel1 != null)
            instructionPanel1.SetActive(false);
        pausePanel.SetActive(false);
        if (lifelineGainedPanel != null)
            lifelineGainedPanel.SetActive(false);
        resultsPanel.SetActive(false);

        // Show instruction panel 2
        if (instructionPanel2 != null)
        {
            instructionPanel2.SetActive(true);
            Debug.Log("Instruction Panel 2 is now active");
        }
        else
        {
            Debug.LogWarning("Instruction Panel 2 is not assigned in the Inspector!");
        }
    }

    private void OnInstructionPanel2NextClicked()
    {
        Debug.Log("Instruction Panel 2 Next Button clicked!");
        
        // Hide instruction panel 2
        if (instructionPanel2 != null)
        {
            instructionPanel2.SetActive(false);
            Debug.Log("Instruction Panel 2 hidden");
        }

        // Show the User Info Panel via UIManager
        if (uiManager != null)
        {
            uiManager.ShowUserInfoPanel();
            Debug.Log("User Info Panel shown via UIManager");
        }
        else
        {
            Debug.LogError("UIManager reference is not assigned in QuizProper Inspector!");
        }
    }

    // === RESULTS NEXT BUTTON (CUTSCENE TRANSITION) ===
    private void OnResultsNextClicked()
    {
        Debug.Log("Results Next Button clicked!");
        
        // Determine which cutscene to load based on score
        string sceneToLoad;
        
        if (currentScore >= cutsceneScoreThreshold)
        {
            sceneToLoad = goodCutsceneSceneName;
            Debug.Log($"Score {currentScore} >= {cutsceneScoreThreshold}: Loading GOOD cutscene: {sceneToLoad}");
        }
        else
        {
            sceneToLoad = badCutsceneSceneName;
            Debug.Log($"Score {currentScore} < {cutsceneScoreThreshold}: Loading BAD cutscene: {sceneToLoad}");
        }

        // Load the appropriate cutscene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
