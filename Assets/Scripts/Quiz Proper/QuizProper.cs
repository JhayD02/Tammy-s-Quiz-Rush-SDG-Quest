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
    [SerializeField] private float resultsPanelFadeDelay = 1.0f;
    [SerializeField] private float resultsPanelFadeDuration = 1.0f;
    
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
    private int correctAnswerStreak = 0; // For streak-based lifeline reward

    // Button position shuffling - remember initial positions for swapping
    private Vector2[] initialButtonPositions = new Vector2[4]; // Store initial anchoredPosition of each button
    private bool positionsInitialized = false;

    // Coroutines
    private Coroutine timerCoroutine;
    private Coroutine fadeInNextButtonCoroutine;

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

        // Store initial button positions for shuffling
        InitializeButtonPositions();

        // Hide UI elements at start
        nextQuestionButton.gameObject.SetActive(false);
        pausePanel.SetActive(false);
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
        isAnswering = true;
        timeRemaining = timePerQuestion;
        UpdateTimerLabel();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCountDown());
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

        // Apply double points if active
        if (hasDoublePoints)
            points *= 2;

        AddScore(points);

        // Check for streak-based lifeline
        if (correctAnswerStreak == 5)
        {
            GiveRandomLifeline();
            correctAnswerStreak = 0; // Reset streak
        }

        // Check for guaranteed lifeline at question 15 and 25
        if (currentQuestionIndex == 14 || currentQuestionIndex == 24)
        {
            GiveRandomLifeline();
        }

        ShowFeedback(question, "CORRECT", question.correctAnswerIndex);
    }

    private void HandleWrongAnswer(QuizQuestion question)
    {
        correctAnswerStreak = 0; // Lose streak
        ShowFeedback(question, "WRONG", question.correctAnswerIndex);
    }

    private void ShowFeedback(QuizQuestion question, string feedbackType, int correctAnswerIndex)
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

        // Show the next button after a delay
        if (fadeInNextButtonCoroutine != null)
            StopCoroutine(fadeInNextButtonCoroutine);
        fadeInNextButtonCoroutine = StartCoroutine(FadeInNextButton());
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

        hasDoublePoints = false;
        // This will be applied when the answer is submitted
        UpdateLifelineButtons();
    }

    private void UseReduceChoices()
    {
        if (!hasReduceChoices || !isAnswering)
            return;

        hasReduceChoices = false;

        QuizQuestion currentQuestion = shuffledQuestions[currentQuestionIndex];
        
        // Find wrong answers and disable one
        List<int> wrongAnswers = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (i != currentQuestion.correctAnswerIndex)
            {
                wrongAnswers.Add(i);
            }
        }

        if (wrongAnswers.Count > 0)
        {
            int buttonToDisable = wrongAnswers[Random.Range(0, wrongAnswers.Count)];
            answerButtons[buttonToDisable].interactable = false;
        }

        UpdateLifelineButtons();
    }

    private void UpdateLifelineButtons()
    {
        stopTimeButton.interactable = hasStopTime && isAnswering;
        doublePointsButton.interactable = hasDoublePoints && isAnswering;
        reduceChoicesButton.interactable = hasReduceChoices && isAnswering;
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
}
