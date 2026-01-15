// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// namespace QuizGame
// {
//     public class QuizGameController : MonoBehaviour
//     {
//         [Header("Data")]
//         [SerializeField] private QuizOnboardingUI onboardingUI;
//         [SerializeField] private List<QuizQuestion> questionBank = new List<QuizQuestion>();
//         [SerializeField] private int questionsPerGame = 30;
//         [SerializeField] private float questionDurationSeconds = 20f;

//         [Header("UI References")]
//         [SerializeField] private TextMeshProUGUI questionOrFeedbackLabel;
//         [SerializeField] private TextMeshProUGUI timerLabel;
//         [SerializeField] private TextMeshProUGUI scoreLabel;
//         [SerializeField] private CanvasGroup nextQuestionCanvasGroup;
//         [SerializeField] private Button nextQuestionButton;
//         [SerializeField] private Button pauseButton;
//         [SerializeField] private GameObject pausePanel;
//         [SerializeField] private Button resumeButton;
//         [SerializeField] private Button restartButton;
//         [SerializeField] private Button homeButton;

//         [Serializable]
//         private class AnswerOptionUI
//         {
//             public Button button;
//             public TextMeshProUGUI textLabel;
//             public Image image;
//             [HideInInspector] public RectTransform rectTransform;
//         }

//         [Header("Answer Buttons (Order: 1,2,3,4)")]
//         [SerializeField] private AnswerOptionUI[] answerButtons = new AnswerOptionUI[4];

//         [Header("Lifeline Buttons")]
//         [SerializeField] private Button stopTimeButton;
//         [SerializeField] private Button doublePointsButton;
//         [SerializeField] private Button reduceChoicesButton;

//         [Header("Animations")]
//         [SerializeField] private float scoreTickInterval = 0.02f;
//         [SerializeField] private float nextButtonFadeDelay = 1.5f;
//         [SerializeField] private float nextButtonFadeDuration = 0.4f;

//         [Header("Events")]
//         [SerializeField] private UnityEvent onQuizFinished;

//         private readonly List<QuizQuestion> _activeQuestions = new List<QuizQuestion>();
//         private readonly List<Vector2> _initialButtonPositions = new List<Vector2>(4);
//         private readonly int[] _displayIndexForButton = new int[4];

//         private int _currentQuestionNumber;
//         private int _currentQuestionIndex; // index within _activeQuestions
//         private bool _questionActive;
//         private float _timeRemaining;
//         private Coroutine _timerRoutine;

//         private int _score;
//         private bool _doublePointsActive;
//         private bool _stopTimeActive;
//         private bool _hasReducedChoicesThisQuestion;
//         private int _correctStreak;

//         private bool _hasStopTime;
//         private bool _hasDoublePoints;
//         private bool _hasReduceChoices;

//         private bool _pendingStopTimer;
//         private bool _quizStarted;
//         private PlayerIdentity _player;

//         private const int PointsFast = 120;
//         private const int PointsMedium = 110;
//         private const int PointsSlow = 100;
//         private const int FastThreshold = 5;
//         private const int MediumThreshold = 12;
//         private const int LifelineBonus = 150;

//         private void Awake()
//         {
//             for (int i = 0; i < answerButtons.Length; i++)
//             {
//                 answerButtons[i].rectTransform = answerButtons[i].button.GetComponent<RectTransform>();
//                 _initialButtonPositions.Add(answerButtons[i].rectTransform.anchoredPosition);
//                 int capturedIndex = i;
//                 answerButtons[i].button.onClick.AddListener(() => HandleAnswerSelected(capturedIndex));
//             }

//             nextQuestionButton.onClick.AddListener(HandleNextQuestionRequested);
//             pauseButton.onClick.AddListener(PauseGame);
//             resumeButton.onClick.AddListener(ResumeGame);
//             restartButton.onClick.AddListener(RestartQuiz);
//             homeButton.onClick.AddListener(LoadHomeScene);

//             stopTimeButton.onClick.AddListener(HandleStopTime);
//             doublePointsButton.onClick.AddListener(HandleDoublePoints);
//             reduceChoicesButton.onClick.AddListener(HandleReduceChoices);

//             UpdateLifelineButtons();
//             pausePanel.SetActive(false);
//             nextQuestionCanvasGroup.alpha = 0f;
//             nextQuestionButton.gameObject.SetActive(false);

//             if (onboardingUI != null)
//             {
//                 onboardingUI.enabled = true;
//                 onboardingUI.OnStartQuiz.AddListener(BeginQuiz);
//             }
//         }

//         private void BeginQuiz(PlayerIdentity player)
//         {
//             if (_quizStarted)
//             {
//                 return;
//             }

//             _quizStarted = true;
//             _player = player;
//             PrepareQuestions();
//             LoadQuestion(0);
//         }

//         private void PrepareQuestions()
//         {
//             _activeQuestions.Clear();
//             _activeQuestions.AddRange(questionBank.OrderBy(_ => UnityEngine.Random.value).Take(questionsPerGame));
//             _currentQuestionIndex = 0;
//             _currentQuestionNumber = 1;
//             _score = 0;
//             _correctStreak = 0;
//             _hasStopTime = false;
//             _hasDoublePoints = false;
//             _hasReduceChoices = false;
//             UpdateScoreLabel();
//             UpdateLifelineButtons();
//         }

//         private void LoadQuestion(int index)
//         {
//             if (index >= _activeQuestions.Count)
//             {
//                 FinishQuiz();
//                 return;
//             }

//             QuizQuestion question = _activeQuestions[index];
//             ApplyAnswerLayoutShuffle();
//             for (int buttonIndex = 0; buttonIndex < answerButtons.Length; buttonIndex++)
//             {
//                 int sourceAnswerIndex = _displayIndexForButton[buttonIndex];
//                 AnswerChoice choice = question.Answers[sourceAnswerIndex];
//                 var ui = answerButtons[buttonIndex];

//                 ui.textLabel.gameObject.SetActive(!choice.IsImage);
//                 ui.image.gameObject.SetActive(choice.IsImage);
//                 ui.textLabel.text = choice.Text;
//                 ui.image.sprite = choice.Image;
//                 ui.button.interactable = true;
//             }

//             questionOrFeedbackLabel.text = question.QuestionText;
//             nextQuestionButton.gameObject.SetActive(false);
//             nextQuestionCanvasGroup.alpha = 0f;
//             _questionActive = true;
//             _stopTimeActive = false;
//             _doublePointsActive = false;
//             _hasReducedChoicesThisQuestion = false;
//             UpdateLifelineButtons();

//             ResetTimer();
//             if (_timerRoutine != null)
//             {
//                 StopCoroutine(_timerRoutine);
//             }
//             _timerRoutine = StartCoroutine(TimerRoutine());
//         }

//         private void ApplyAnswerLayoutShuffle()
//         {
//             // Start with identity mapping
//             for (int i = 0; i < _displayIndexForButton.Length; i++)
//             {
//                 _displayIndexForButton[i] = i;
//             }

//             bool swap13 = UnityEngine.Random.value < 0.5f;
//             bool swap24 = UnityEngine.Random.value < 0.5f;

//             if (swap13)
//             {
//                 Swap(ref _displayIndexForButton[0], ref _displayIndexForButton[2]);
//                 SwapPositions(answerButtons[0], answerButtons[2]);
//             }
//             else
//             {
//                 ResetPosition(answerButtons[0], 0);
//                 ResetPosition(answerButtons[2], 2);
//             }

//             if (swap24)
//             {
//                 Swap(ref _displayIndexForButton[1], ref _displayIndexForButton[3]);
//                 SwapPositions(answerButtons[1], answerButtons[3]);
//             }
//             else
//             {
//                 ResetPosition(answerButtons[1], 1);
//                 ResetPosition(answerButtons[3], 3);
//             }

//             // Ensure positions reset when neither swap occurs
//             if (!swap13 && !swap24)
//             {
//                 for (int i = 0; i < answerButtons.Length; i++)
//                 {
//                     ResetPosition(answerButtons[i], i);
//                 }
//             }
//         }

//         private void ResetPosition(AnswerOptionUI ui, int originalIndex)
//         {
//             ui.rectTransform.anchoredPosition = _initialButtonPositions[originalIndex];
//         }

//         private void SwapPositions(AnswerOptionUI a, AnswerOptionUI b)
//         {
//             Vector2 posA = a.rectTransform.anchoredPosition;
//             a.rectTransform.anchoredPosition = b.rectTransform.anchoredPosition;
//             b.rectTransform.anchoredPosition = posA;
//         }

//         private void ResetTimer()
//         {
//             _timeRemaining = questionDurationSeconds;
//             UpdateTimerLabel();
//         }

//         private IEnumerator TimerRoutine()
//         {
//             while (_timeRemaining > 0f && _questionActive && !_stopTimeActive)
//             {
//                 _timeRemaining -= Time.deltaTime;
//                 UpdateTimerLabel();
//                 yield return null;
//             }

//             if (_questionActive && !_stopTimeActive)
//             {
//                 HandleTimeExpired();
//             }
//         }

//         private void UpdateTimerLabel()
//         {
//             timerLabel.text = Mathf.CeilToInt(Mathf.Max(0f, _timeRemaining)).ToString();
//         }

//         private void HandleAnswerSelected(int buttonIndex)
//         {
//             if (!_questionActive)
//             {
//                 return;
//             }

//             _questionActive = false;
//             if (_timerRoutine != null)
//             {
//                 StopCoroutine(_timerRoutine);
//             }

//             QuizQuestion question = _activeQuestions[_currentQuestionIndex];
//             int chosenAnswerIndex = _displayIndexForButton[buttonIndex];
//             bool isCorrect = chosenAnswerIndex == question.CorrectAnswerIndex;
//             float timeUsed = questionDurationSeconds - _timeRemaining;

//             if (isCorrect)
//             {
//                 int points = CalculateScore(timeUsed);
//                 if (_doublePointsActive)
//                 {
//                     points *= 2;
//                 }

//                 _correctStreak++;
//                 GrantStreakLifelineIfEligible();
//                 AddScore(points);
//                 ShowFeedback(true, question);
//             }
//             else
//             {
//                 _correctStreak = 0;
//                 ShowFeedback(false, question);
//             }
//         }

//         private void HandleTimeExpired()
//         {
//             _questionActive = false;
//             _correctStreak = 0;
//             ShowTimeUpFeedback();
//         }

//         private void ShowFeedback(bool correct, QuizQuestion question)
//         {
//             string correctAnswerText = question.Answers[question.CorrectAnswerIndex].Text;
//             string feedback;
//             if (correct)
//             {
//                 feedback = $"Correct! The answer is {correctAnswerText}.\nBecause {question.FeedbackExplanation}";
//             }
//             else
//             {
//                 feedback = $"Incorrect. The correct answer is {correctAnswerText}.\nBecause {question.FeedbackExplanation}";
//             }

//             questionOrFeedbackLabel.text = feedback;
//             RevealNextButton();
//         }

//         private void ShowTimeUpFeedback()
//         {
//             QuizQuestion question = _activeQuestions[_currentQuestionIndex];
//             string correctAnswerText = question.Answers[question.CorrectAnswerIndex].Text;
//             string feedback = $"TIME'S UP! The answer is {correctAnswerText}.\nBecause {question.FeedbackExplanation}";
//             questionOrFeedbackLabel.text = feedback;
//             RevealNextButton();
//         }

//         private void RevealNextButton()
//         {
//             nextQuestionButton.gameObject.SetActive(true);
//             StartCoroutine(FadeInNextButton());
//         }

//         private IEnumerator FadeInNextButton()
//         {
//             nextQuestionCanvasGroup.alpha = 0f;
//             yield return new WaitForSeconds(nextButtonFadeDelay);

//             float elapsed = 0f;
//             nextQuestionCanvasGroup.blocksRaycasts = true;
//             while (elapsed < nextButtonFadeDuration)
//             {
//                 elapsed += Time.deltaTime;
//                 nextQuestionCanvasGroup.alpha = Mathf.Clamp01(elapsed / nextButtonFadeDuration);
//                 yield return null;
//             }

//             nextQuestionCanvasGroup.alpha = 1f;
//         }

//         private void HandleNextQuestionRequested()
//         {
//             _currentQuestionIndex++;
//             _currentQuestionNumber++;
//             ClearLifelineStatePerQuestion();
//             EnsureGuaranteeLifeline();
//             LoadQuestion(_currentQuestionIndex);
//         }

//         private void ClearLifelineStatePerQuestion()
//         {
//             _doublePointsActive = false;
//             _stopTimeActive = false;
//             _hasReducedChoicesThisQuestion = false;
//             UpdateLifelineButtons();
//         }

//         private void GrantStreakLifelineIfEligible()
//         {
//             if (_correctStreak == 5)
//             {
//                 GrantRandomAvailableLifeline();
//                 _correctStreak = 0;
//             }
//         }

//         private void EnsureGuaranteeLifeline()
//         {
//             if (_currentQuestionNumber == 15 || _currentQuestionNumber == 25)
//             {
//                 GrantRandomAvailableLifeline();
//             }
//         }

//         private void GrantRandomAvailableLifeline()
//         {
//             List<int> candidates = new List<int>();
//             if (!_hasStopTime) candidates.Add(0);
//             if (!_hasDoublePoints) candidates.Add(1);
//             if (!_hasReduceChoices) candidates.Add(2);

//             if (candidates.Count == 0)
//             {
//                 return;
//             }

//             int chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
//             switch (chosen)
//             {
//                 case 0:
//                     _hasStopTime = true;
//                     break;
//                 case 1:
//                     _hasDoublePoints = true;
//                     break;
//                 case 2:
//                     _hasReduceChoices = true;
//                     break;
//             }

//             UpdateLifelineButtons();
//         }

//         private void UpdateLifelineButtons()
//         {
//             stopTimeButton.interactable = _hasStopTime && !_stopTimeActive;
//             doublePointsButton.interactable = _hasDoublePoints && !_doublePointsActive;
//             reduceChoicesButton.interactable = _hasReduceChoices && !_hasReducedChoicesThisQuestion;
//         }

//         private void HandleStopTime()
//         {
//             if (!_hasStopTime || !_questionActive)
//             {
//                 return;
//             }

//             _stopTimeActive = true;
//             _hasStopTime = false;
//             UpdateLifelineButtons();
//         }

//         private void HandleDoublePoints()
//         {
//             if (!_hasDoublePoints || !_questionActive)
//             {
//                 return;
//             }

//             _doublePointsActive = true;
//             _hasDoublePoints = false;
//             UpdateLifelineButtons();
//         }

//         private void HandleReduceChoices()
//         {
//             if (!reduceChoicesButton.interactable || _hasReducedChoicesThisQuestion)
//             {
//                 return;
//             }

//             _hasReducedChoicesThisQuestion = true;
//             reduceChoicesButton.interactable = false;

//             QuizQuestion question = _activeQuestions[_currentQuestionIndex];
//             List<int> wrongIndices = Enumerable.Range(0, 4).Where(i => i != question.CorrectAnswerIndex).ToList();
//             wrongIndices = wrongIndices.OrderBy(_ => UnityEngine.Random.value).ToList();
//             int keepWrongIndex = wrongIndices[0];
//             int disableWrongIndex = wrongIndices[1];

//             for (int buttonIndex = 0; buttonIndex < answerButtons.Length; buttonIndex++)
//             {
//                 int sourceIndex = _displayIndexForButton[buttonIndex];
//                 if (sourceIndex == disableWrongIndex)
//                 {
//                     answerButtons[buttonIndex].button.interactable = false;
//                 }
//             }
//         }

//         private int CalculateScore(float timeUsed)
//         {
//             if (timeUsed <= FastThreshold)
//             {
//                 return PointsFast;
//             }

//             if (timeUsed <= MediumThreshold)
//             {
//                 return PointsMedium;
//             }

//             return PointsSlow;
//         }

//         private void AddScore(int points)
//         {
//             int start = _score;
//             int target = _score + points;
//             _score = target;
//             StartCoroutine(AnimateScore(start, target));
//         }

//         private IEnumerator AnimateScore(int from, int to)
//         {
//             int current = from;
//             while (current < to)
//             {
//                 current++;
//                 scoreLabel.text = current.ToString();
//                 yield return new WaitForSeconds(scoreTickInterval);
//             }
//         }

//         private void UpdateScoreLabel()
//         {
//             scoreLabel.text = _score.ToString();
//         }

//         private void PauseGame()
//         {
//             if (!_quizStarted)
//             {
//                 return;
//             }

//             _pendingStopTimer = _questionActive;
//             _questionActive = false;
//             if (_timerRoutine != null)
//             {
//                 StopCoroutine(_timerRoutine);
//             }

//             pausePanel.SetActive(true);
//             Time.timeScale = 0f;
//         }

//         private void ResumeGame()
//         {
//             pausePanel.SetActive(false);
//             Time.timeScale = 1f;
//             if (_pendingStopTimer)
//             {
//                 _pendingStopTimer = false;
//                 _questionActive = true;
//                 if (_timerRoutine != null)
//                 {
//                     StopCoroutine(_timerRoutine);
//                 }
//                 _timerRoutine = StartCoroutine(TimerRoutine());
//             }
//         }

//         private void RestartQuiz()
//         {
//             Time.timeScale = 1f;
//             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//         }

//         private void LoadHomeScene()
//         {
//             Time.timeScale = 1f;
//             SceneManager.LoadScene("Home");
//         }

//         private void FinishQuiz()
//         {
//             int unusedLifelines = CountUnusedLifelines();
//             int bonus = unusedLifelines * LifelineBonus;
//             AddScore(bonus);
//             SaveSession();
//             onQuizFinished?.Invoke();
//         }

//         private int CountUnusedLifelines()
//         {
//             int count = 0;
//             if (stopTimeButton.interactable) count++;
//             if (doublePointsButton.interactable) count++;
//             if (reduceChoicesButton.interactable) count++;
//             return count;
//         }

//         private void SaveSession()
//         {
//             PlayerSessionRecord record = new PlayerSessionRecord
//             {
//                 FirstName = _player.FirstName,
//                 LastName = _player.LastName,
//                 School = _player.SchoolResolved,
//                 FinalScore = _score,
//                 Timestamp = DateTime.Now.ToString("MM/dd/yyyy")
//             };

//             SessionPersistence.AppendSession(record);
//         }
//     }
// }
