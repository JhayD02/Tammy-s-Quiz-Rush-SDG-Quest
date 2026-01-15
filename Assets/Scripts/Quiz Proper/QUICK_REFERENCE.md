# QUICK REFERENCE CARD

## The 4 Scripts at a Glance

| Script | Purpose | Attach To | What It Does |
|--------|---------|-----------|--------------|
| **QuestionData.cs** | Question structure | Nothing (just data) | Defines how questions should be formatted |
| **UIManager.cs** | Onboarding screens | UIManager GameObject | Shows intro, collects player name & school |
| **QuizProper.cs** | Main game logic | QuizProper GameObject | Runs the quiz, manages timer, scoring, lifelines |
| **PlayerManager.cs** | Save system | PlayerManager GameObject | Saves player scores to device locally |

---

## Inspector Setup - The Essential Fields

### UIManager Inspector

```
Instruction Panel → InstructionPanel GameObject
Instruction Label → InstructionLabel Text
Instruction Text → Type your welcome message
Next From Instruction Button → NextButton

First Name Input → FirstNameInput (TMP InputField)
Last Name Input → LastNameInput (TMP InputField)
School Buttons (4) → TechButton, AlabangsButton, DilimanButton, OthersButton
Others Abbreviation Container → OthersAbbreviationContainer
Abbreviation Input → AbbreviationInput

Start Quiz Button → StartQuizButton

Selected Button Color → Light blue RGB(51, 153, 255)
Normal Button Color → White RGB(255, 255, 255)

Quiz Proper → QuizProper GameObject
```

### QuizProper Inspector

```
Question Bank → 30+ questions (fill in later)
Total Questions In Game → 30
Time Per Question → 20 (seconds)

Question Label → QuestionLabel Text
Timer Label → TimerLabel Text
Score Label → ScoreLabel Text

Answer Buttons (4) → AnswerButton1, AnswerButton2, AnswerButton3, AnswerButton4
Answer Texts (4) → Text component of each button
Answer Images (4) → Image component of each button

Next Question Button → NextQuestionButton
Next Button Canvas Group → CanvasGroup component of NextQuestionButton
Pause Button → PauseButton
Pause Panel → PausePanel

Resume Button → ResumeButton
Restart Button → RestartButton
Home Button → HomeButton

Stop Time Button → StopTimeButton
Double Points Button → DoublePointsButton
Reduce Choices Button → ReduceChoicesButton

Next Button Fade Delay → 1.5
Next Button Fade Duration → 0.4
Score Increment Speed → 0.02

Points Fast Answer → 120 (≤5 seconds)
Points Medium Answer → 110 (6-12 seconds)
Points Slow Answer → 100 (13+ seconds)
Lifeline Bonus → 150

Correct Answer Template → Correct! The answer is [ANSWER]. It is because [REASON]
Wrong Answer Template → Incorrect. The correct answer is [ANSWER]. It is because [REASON]
Time Up Template → TIME'S UP! The correct answer is [ANSWER]. It is because [REASON]
```

### PlayerManager Inspector

```
(No fields to fill - just needs "Do Not Destroy On Load" checked)
```

---

## File Structure

```
Assets/
├── Scripts/
│   └── Quiz Proper/
│       ├── QuestionData.cs
│       ├── UIManager.cs
│       ├── QuizProper.cs
│       ├── PlayerManager.cs
│       ├── COMPLETE_SETUP_GUIDE.md
│       ├── SETUP_GUIDE.md
│       ├── BUTTON_SHUFFLING_EXPLAINED.md
│       └── HOW_TO_ADD_QUESTIONS.md
├── Scenes/
│   └── QuizScene.unity
└── ... (other assets)
```

---

## Scene Hierarchy (What You Should See)

```
Canvas
├── UIManager (GameObject with UIManager script)
├── InstructionPanel (UI Panel)
│   ├── InstructionLabel (TextMeshProUGUI)
│   └── NextButton (Button)
├── UserInfoPanel (UI Panel - starts inactive)
│   ├── FirstNameInput (TMP InputField)
│   ├── LastNameInput (TMP InputField)
│   ├── SchoolButtonsContainer
│   │   ├── TechButton (Button)
│   │   ├── AlabangsButton (Button)
│   │   ├── DilimanButton (Button)
│   │   └── OthersButton (Button)
│   ├── OthersAbbreviationContainer (starts inactive)
│   │   └── AbbreviationInput (TMP InputField)
│   └── StartQuizButton (Button - starts inactive)
├── QuizContentPanel (UI Panel)
│   ├── QuestionLabel (TextMeshProUGUI)
│   ├── TimerLabel (TextMeshProUGUI)
│   ├── ScoreLabel (TextMeshProUGUI)
│   ├── AnswerButtonsContainer
│   │   ├── AnswerButton1 (Button with Image)
│   │   ├── AnswerButton2 (Button with Image)
│   │   ├── AnswerButton3 (Button with Image)
│   │   └── AnswerButton4 (Button with Image)
│   ├── LifelinesContainer
│   │   ├── StopTimeButton (Button)
│   │   ├── DoublePointsButton (Button)
│   │   └── ReduceChoicesButton (Button)
│   ├── NextQuestionButton (Button - starts inactive)
│   └── PauseButton (Button)
└── PausePanel (UI Panel - starts inactive)
    ├── ResumeButton (Button)
    ├── RestartButton (Button)
    └── HomeButton (Button)

QuizProper (GameObject with QuizProper script)
PlayerManager (GameObject with PlayerManager script)
```

---

## How the Game Flows

```
1. Play
   ↓
2. InstructionPanel shows
   ↓
3. Player clicks "Next"
   ↓
4. UserInfoPanel shows
   ↓
5. Player enters name and school
   ↓
6. "START QUIZ" appears (when form valid)
   ↓
7. Player clicks "START QUIZ"
   ↓
8. UIManager → PlayerManager.SetPlayerInfo()
9. UIManager → QuizProper.StartQuiz()
   ↓
10. QuizProper shuffles questions
    ↓
11. QuizProper shows Question 1
    ↓
12. 20-second timer starts
    ↓
13. Player clicks answer (or timer expires)
    ↓
14. QuizProper checks answer
    ↓
15. Feedback shown for 1.5 seconds
    ↓
16. "Next Question" button fades in
    ↓
17. Player clicks "Next Question"
    ↓
18. Repeat steps 11-17 for questions 2-30
    ↓
19. After Question 30:
    - Add lifeline bonuses
    - Call PlayerManager.SetFinalScore()
    - PlayerManager saves to JSON file
```

---

## Common Values to Adjust

These are all in the **QuizProper Inspector**:

| Setting | Default | What It Does | Good Range |
|---------|---------|------------|------------|
| Time Per Question | 20 | Seconds per question | 15-30 |
| Points Fast Answer | 120 | Points for ≤5 seconds | 100-150 |
| Points Medium Answer | 110 | Points for 6-12 seconds | 80-130 |
| Points Slow Answer | 100 | Points for 13+ seconds | 50-100 |
| Lifeline Bonus | 150 | Bonus per unused lifeline | 100-200 |
| Next Button Fade Delay | 1.5 | Seconds before Next appears | 0.5-2 |
| Score Increment Speed | 0.02 | How fast score counts up | 0.01-0.05 |

---

## Lifeline System

### How to Get Lifelines

**Earned:**
- After 5 correct answers in a row → Get a random lifeline
- After 5 correct answers → Reset streak counter

**Guaranteed:**
- After Question 15 → Get a random lifeline
- After Question 25 → Get a random lifeline

**Bonuses:**
- After Question 30 → +150 points per unused lifeline

### What Each Lifeline Does

1. **Stop Time**
   - Stops the timer from counting down
   - Single use per question
   - Can be used while answering

2. **Double Points**
   - Next correct answer gives 2x points
   - If correct in ≤5s normally = 120 → becomes 240
   - Single use per question

3. **Reduce Choices**
   - Disables one wrong answer (makes 3 clickable buttons)
   - Single use per question
   - Can only use once per question even if you have it

---

## Answer Shuffling (The Button Swap)

**Each question:**
- Buttons 1 and 3 randomly swap positions (50% chance)
- Buttons 2 and 4 randomly swap positions (50% chance)
- The correct answer is still correct, just in a different position

**Why?** So players can't just click the same button every time.

**Example:**
```
Normal order: Answer A(1) B(2) C(3) D(4), correct=2
After shuffle: Answer C(1) B(2) A(3) D(4), correct=2
So Button 2 still has the correct answer!
```

---

## Scoring Breakdown

### For Correct Answers

| Speed | Points |
|-------|--------|
| ≤5 seconds | 120 |
| 6-12 seconds | 110 |
| 13+ seconds | 100 |
| *2 (if Double Points active) | x2 |

### For Wrong/Time-Up Answers
- 0 points

### End Game Bonus
- +150 points per unused lifeline (max 450 for all 3)

---

## Player Save File

**Location:** 
```
C:\Users\[YourUsername]\AppData\LocalLow\DefaultCompany\[GameName]\player_scores.json
```

**Format:**
```json
{
  "allRecords": [
    {
      "firstName": "John",
      "lastName": "Doe",
      "school": "TECH",
      "finalScore": 3450,
      "timestamp": "01/15/2026"
    }
  ]
}
```

---

## Useful PlayerManager Functions

```csharp
// Get player info
string name = PlayerManager.Instance.GetPlayerFirstName();
string school = PlayerManager.Instance.GetPlayerSchool();

// Get leaderboards (useful for later)
List<PlayerRecord> topLocal = PlayerManager.Instance.GetTopLocalScores(5);
List<PlayerRecord> topSchool = PlayerManager.Instance.GetTopScoresForSchool("TECH", 5);
```

---

## Troubleshooting Checklist

### Game Won't Start
- [ ] All 4 scripts created?
- [ ] Scripts attached to correct GameObjects?
- [ ] Canvas exists in scene?
- [ ] Question Bank has at least 1 question?

### Name/School Fields Don't Work
- [ ] Using TMP InputField (not regular InputField)?
- [ ] Fields assigned to UIManager?
- [ ] School buttons in correct order (TECH, ALABANG, DILIMAN, OTHERS)?

### Questions Don't Show
- [ ] Question Bank size > 0?
- [ ] First question has all fields filled?
- [ ] Correct Answer Index is 0-3?

### Score Doesn't Update
- [ ] ScoreLabel assigned to QuizProper?
- [ ] Answering correctly (not getting 0 points)?

### Scores Not Saving
- [ ] PlayerManager GameObject exists?
- [ ] PlayerManager set to "Do Not Destroy On Load"?
- [ ] Completed full 30 questions?

---

## Next Steps (For Future Development)

1. **Leaderboard UI** - Display saved scores
2. **LootLocker Integration** - Upload scores online
3. **Sound Effects** - Add audio feedback
4. **Animations** - Polish the UI with transitions
5. **Difficulty Levels** - Different question sets

But first, **get this system working perfectly!** 

---

## Where to Find Help

- **General Setup** → COMPLETE_SETUP_GUIDE.md
- **Script Explanations** → SETUP_GUIDE.md
- **Button Shuffling** → BUTTON_SHUFFLING_EXPLAINED.md
- **Adding Questions** → HOW_TO_ADD_QUESTIONS.md
- **Quick Answers** → This document

---

**You've got all the tools you need. Now go build! 🚀**
