# COMPLETE QUIZ SYSTEM - STEP-BY-STEP SETUP GUIDE

## Overview

You have **4 scripts** and **4 guidance documents**. This guide walks you through EVERYTHING from start to finish.

---

## STEP 0: Understanding What You Have

### The 4 Scripts (Copy-Paste Ready)

1. **QuestionData.cs** - Defines question structure
2. **UIManager.cs** - Handles onboarding (name, school)
3. **QuizProper.cs** - Runs the actual quiz
4. **PlayerManager.cs** - Saves scores locally

### The 4 Guidance Documents

1. **SETUP_GUIDE.md** - Overview of all scripts
2. **BUTTON_SHUFFLING_EXPLAINED.md** - How answer shuffling works
3. **HOW_TO_ADD_QUESTIONS.md** - How to add questions in Inspector
4. **This document** - Complete step-by-step walkthrough

---

## PHASE 1: CREATE YOUR SCENE (5 minutes)

### Step 1.1: Create a New Scene
1. Right-click in Project folder
2. Create > Scene
3. Name it **"QuizScene"**
4. Save it in **Assets/Scenes/**

### Step 1.2: Create the Canvas
1. Right-click in Hierarchy
2. UI > Canvas
3. Set **Canvas Scaler** to "Scale With Screen Size"
4. **ANDROID MOBILE**: Add **SafeAreaLayoutGroup** component
   - Click **Add Component** on Canvas
   - Search "SafeAreaLayoutGroup" and add it
   - This prevents UI from being cut off by notches on Android phones

This is where all your UI will live.

---

## PHASE 2: CREATE THE SCRIPTS (10 minutes)

### Step 2.1: Create Folder
1. Right-click in Assets/Scripts
2. Create Folder
3. Name it **"Quiz Proper"**

### Step 2.2: Create 4 Scripts
1. Right-click in the new folder
2. Create > C# Script
3. Name: **QuestionData**
4. Copy the QuestionData.cs code into it
5. Repeat for:
   - **UIManager.cs**
   - **QuizProper.cs**
   - **PlayerManager.cs**

**Save each file after pasting!**

---

## PHASE 3: CREATE INSTRUCTION PANEL UI (15 minutes)

### Step 3.1: Create Instruction Panel
1. Right-click on Canvas
2. UI > Panel
3. Name: **InstructionPanel**
4. Set Anchors to "Stretch" (full screen)

### Step 3.2: Add Instruction Label
1. Right-click on InstructionPanel
2. UI > Text - TextMeshPro
3. Name: **InstructionLabel**
4. Set text to: "Welcome to the Quiz!\n\nAnswer 30 questions to test your knowledge."
5. Adjust size and font

### Step 3.3: Add Next Button
1. Right-click on InstructionPanel
2. UI > Button - TextMeshPro
3. Name: **NextButton**
4. Change text to: "Next"
5. Position it at the bottom center

---

## PHASE 4: CREATE USER INFO PANEL UI (20 minutes)

### Step 4.1: Create User Info Panel
1. Right-click on Canvas
2. UI > Panel
3. Name: **UserInfoPanel**
4. Set Anchors to "Stretch"
5. **UNCHECK "Active"** (it should start invisible)

### Step 4.2: Add Name Inputs
1. Right-click on UserInfoPanel
2. UI > InputField - TextMeshPro
3. Name: **FirstNameInput**
4. Set placeholder text to: "Enter First Name"
5. Repeat for **LastNameInput** ("Enter Last Name")

Position them vertically.

### Step 4.3: Create School Buttons Container
1. Right-click on UserInfoPanel
2. Create Empty
3. Name: **SchoolButtonsContainer**
4. Position it in the middle area

### Step 4.4: Create 4 School Buttons (IN THIS ORDER!)

**Important:** Order matters! They must be TECH, ALABANG, DILIMAN, OTHERS.

1. Right-click SchoolButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **TechButton**
4. Text: "TECH"
5. Position: Top-left

1. Right-click SchoolButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **AlabangsButton**
4. Text: "ALABANG"
5. Position: Top-right

1. Right-click SchoolButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **DilimanButton**
4. Text: "DILIMAN"
5. Position: Bottom-left

1. Right-click SchoolButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **OthersButton**
4. Text: "OTHERS"
5. Position: Bottom-right

### Step 4.5: Create Others Abbreviation Container
1. Right-click on UserInfoPanel
2. Create Empty
3. Name: **OthersAbbreviationContainer**
4. **UNCHECK "Active"** (only shows when OTHERS selected)

### Step 4.6: Add Abbreviation Input
1. Right-click OthersAbbreviationContainer
2. UI > InputField - TextMeshPro
3. Name: **AbbreviationInput**
4. Placeholder: "School Code (Max 10)"

### Step 4.7: Add Start Quiz Button
1. Right-click on UserInfoPanel
2. UI > Button - TextMeshPro
3. Name: **StartQuizButton**
4. Text: "START QUIZ"
5. **UNCHECK "Active"** (appears when form is valid)
6. Position at bottom center

---

## PHASE 5: CREATE QUIZ UI (25 minutes)

### Step 5.1: Create Quiz Content Area
1. Right-click on Canvas
2. UI > Panel
3. Name: **QuizContentPanel**
4. Set Anchors to "Stretch"

### Step 5.2: Create Question Display Area
1. Right-click on QuizContentPanel
2. UI > Text - TextMeshPro
3. Name: **QuestionLabel**
4. Position at top
5. Set font size to 40
6. Alignment: Center

### Step 5.3: Create Timer Display
1. Right-click on QuizContentPanel
2. UI > Text - TextMeshPro
3. Name: **TimerLabel**
4. Text: "20"
5. Position at top-right
6. Set font size to 60 (large, very visible)
7. Color: Red (so it's noticeable)

### Step 5.4: Create Score Display
1. Right-click on QuizContentPanel
2. UI > Text - TextMeshPro
3. Name: **ScoreLabel**
4. Text: "Score: 0"
5. Position at top-left
6. Set font size to 30

### Step 5.5: Create Answer Buttons Container
1. Right-click on QuizContentPanel
2. Create Empty
3. Name: **AnswerButtonsContainer**
4. Position in the middle

### Step 5.6: Create 4 Answer Buttons (IN ORDER: 1, 2, 3, 4)

You'll need to create them **vertically stacked**. Make sure they're in this exact order (top to bottom):

**Button 1:**
1. Right-click AnswerButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **AnswerButton1**
4. Text: "Answer 1"
5. Position: Top (Y = 150)
6. Add a Text child (already there) - this shows the answer text
7. Add an Image component - this shows images when needed

**Button 2:**
1. Right-click AnswerButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **AnswerButton2**
4. Text: "Answer 2"
5. Position: Second (Y = 50)

**Button 3:**
1. Right-click AnswerButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **AnswerButton3**
4. Text: "Answer 3"
5. Position: Third (Y = -50)

**Button 4:**
1. Right-click AnswerButtonsContainer
2. UI > Button - TextMeshPro
3. Name: **AnswerButton4**
4. Text: "Answer 4"
5. Position: Bottom (Y = -150)

**Important:** Space buttons at least 100 units apart vertically for the position shuffling to work properly!

### Step 5.7: Create Lifeline Buttons Container
1. Right-click on QuizContentPanel
2. Create Empty
3. Name: **LifelinesContainer**
4. Position at bottom

### Step 5.8: Create 3 Lifeline Buttons

**Stop Time Button:**
1. Right-click LifelinesContainer
2. UI > Button - TextMeshPro
3. Name: **StopTimeButton**
4. Text: "Stop Time"
5. Position: Left

**Double Points Button:**
1. Right-click LifelinesContainer
2. UI > Button - TextMeshPro
3. Name: **DoublePointsButton**
4. Text: "2x Points"
5. Position: Center

**Reduce Choices Button:**
1. Right-click LifelinesContainer
2. UI > Button - TextMeshPro
3. Name: **ReduceChoicesButton**
4. Text: "Remove Wrong"
5. Position: Right

### Step 5.9: Create Next Question Button
1. Right-click on QuizContentPanel
2. UI > Button - TextMeshPro
3. Name: **NextQuestionButton**
4. Text: "Next Question"
5. Position: Bottom-Center
6. **UNCHECK "Active"** (shows after feedback)
7. Add **CanvasGroup** component (Select button, Add Component, search "CanvasGroup")

### Step 5.10: Create Pause Button
1. Right-click on QuizContentPanel
2. UI > Button - TextMeshPro
3. Name: **PauseButton**
4. Text: "Pause"
5. Position: Top-Right corner

### Step 5.11: Create Pause Panel
1. Right-click on Canvas
2. UI > Panel
3. Name: **PausePanel**
4. Set to cover full screen
5. **UNCHECK "Active"** (hidden by default)
6. Make background darker (change image color)

### Step 5.12: Add Pause Buttons
Inside PausePanel, create 3 buttons:

**Resume Button:**
1. UI > Button - TextMeshPro
2. Name: **ResumeButton**
3. Text: "Resume"

**Restart Button:**
1. UI > Button - TextMeshPro
2. Name: **RestartButton**
3. Text: "Restart Quiz"

**Home Button:**
1. UI > Button - TextMeshPro
2. Name: **HomeButton**
3. Text: "Home"

Position them vertically in the center of the pause panel.

---

## PHASE 6: CREATE GAMEOBJECTS AND ATTACH SCRIPTS (10 minutes)

### Step 6.1: Create UIManager GameObject
1. Right-click on Canvas
2. Create Empty
3. Name: **UIManager**
4. Add Component > UIManager (the script)

### Step 6.2: Create QuizProper GameObject
1. Right-click in Hierarchy (not on Canvas)
2. Create Empty
3. Name: **QuizProper**
4. Add Component > QuizProper (the script)

### Step 6.3: Create PlayerManager GameObject
1. Right-click in Hierarchy
2. Create Empty
3. Name: **PlayerManager**
4. Add Component > PlayerManager
5. **IMPORTANT:** Select "Do Not Destroy On Load" checkbox in script properties
   - This keeps the player data even when changing scenes

---

## PHASE 7: WIRE UP THE INSPECTOR (30 minutes)

This is the tedious part but CRITICAL!

### Step 7.1: Wire UIManager

Select the **UIManager** GameObject:

**Instruction Panel Section:**
- Drag **InstructionPanel** into "Instruction Panel" field
- Drag **InstructionLabel** into "Instruction Label" field
- Drag **NextButton** into "Next From Instruction Button" field
- Type in "Instruction Text" field:
  ```
  Welcome to Tammy's Quiz Rush!
  
  Answer 30 questions to test your knowledge.
  Be quick - answering fast earns more points!
  Use lifelines strategically.
  
  Good luck!
  ```

**User Info Panel Section:**
- Drag **UserInfoPanel** into "User Info Panel" field
- Drag **FirstNameInput** into "First Name Input" field
- Drag **LastNameInput** into "Last Name Input" field
- **School Buttons** - Click "Size" and set to 4:
  - Element 0 → Drag **TechButton**
  - Element 1 → Drag **AlabangsButton**
  - Element 2 → Drag **DilimanButton**
  - Element 3 → Drag **OthersButton**
- Drag **OthersAbbreviationContainer** into "Others Abbreviation Container" field
- Drag **AbbreviationInput** into "Abbreviation Input" field
- Drag **StartQuizButton** into "Start Quiz Button" field

**Colors Section:**
- Selected Button Color: RGB(51, 153, 255) - Light blue
- Normal Button Color: RGB(255, 255, 255) - White

**References Section:**
- Drag **QuizProper** GameObject into "Quiz Proper" field

### Step 7.2: Wire QuizProper

Select the **QuizProper** GameObject:

**Data Section:**
- Question Bank: Set size to 30 (or however many questions you have)
  - We'll fill this next
- Total Questions In Game: 30
- Time Per Question: 20

**UI Elements Section:**
- Question Label → Drag **QuestionLabel**
- Timer Label → Drag **TimerLabel**
- Score Label → Drag **ScoreLabel**
- Answer Buttons (4):
  - Element 0 → **AnswerButton1**
  - Element 1 → **AnswerButton2**
  - Element 2 → **AnswerButton3**
  - Element 3 → **AnswerButton4**
- Answer Texts (4):
  - Element 0 → Text component of **AnswerButton1** (select button, find its Text child)
  - Element 1 → Text of **AnswerButton2**
  - Element 2 → Text of **AnswerButton3**
  - Element 3 → Text of **AnswerButton4**
- Answer Images (4):
  - Element 0 → Image component of **AnswerButton1**
  - Element 1 → Image of **AnswerButton2**
  - Element 2 → Image of **AnswerButton3**
  - Element 3 → Image of **AnswerButton4**
- Next Question Button → Drag **NextQuestionButton**
- Next Button Canvas Group → Select **NextQuestionButton**, find CanvasGroup component, assign it here
- Pause Button → Drag **PauseButton**
- Pause Panel → Drag **PausePanel**
- Resume Button → Drag **ResumeButton**
- Restart Button → Drag **RestartButton**
- Home Button → Drag **HomeButton**

**Lifeline Buttons:**
- Stop Time Button → Drag **StopTimeButton**
- Double Points Button → Drag **DoublePointsButton**
- Reduce Choices Button → Drag **ReduceChoicesButton**

**Animations:**
- Next Button Fade Delay: 1.5
- Next Button Fade Duration: 0.4
- Score Increment Speed: 0.02

**Scoring:**
- Points Fast Answer: 120
- Points Medium Answer: 110
- Points Slow Answer: 100
- Lifeline Bonus: 150

**Feedback Templates** (Customizable! Change these to customize player feedback):
- Correct Answer Template: `Correct! The answer is [ANSWER]. It is because [REASON]`
- Wrong Answer Template: `Incorrect. The correct answer is [ANSWER]. It is because [REASON]`
- Time Up Template: `TIME'S UP! The correct answer is [ANSWER]. It is because [REASON]`

**Note:** `[ANSWER]` and `[REASON]` are placeholders:
- `[ANSWER]` → Replaced with the correct answer text
- `[REASON]` → Replaced with the question's feedback explanation

---

## PHASE 8: ADD QUESTIONS (30-45 minutes)

See the **"HOW_TO_ADD_QUESTIONS.md"** document for detailed instructions.

Quick summary:
1. Select **QuizProper** GameObject
2. In Inspector, find **Question Bank**
3. Set Size to 30
4. For each Element (0-29):
   - Fill: Question Text
   - Fill: 4 Answers (size 4)
   - Set: Correct Answer Index (0-3)
   - Fill: Feedback Explanation

---

## PHASE 9: TEST! (5 minutes)

### Step 9.1: Play the Game
1. Click **Play** button
2. You should see the Instruction Panel
3. Click "Next"
4. You should see the User Info Panel
5. Fill in:
   - First Name: "Test"
   - Last Name: "Player"
   - Select a school (e.g., "TECH")
6. "START QUIZ" button should appear
7. Click it
8. You should see Question 1 with 4 answer buttons
9. Click an answer
10. You should see feedback for 1.5 seconds
11. "Next Question" button fades in
12. Click it to go to next question

### Step 9.2: Troubleshooting

**"Instruction Panel doesn't show"**
- Check that InstructionPanel GameObject is active (has checkmark)
- Check that UIManager is assigned correctly

**"Name inputs don't work"**
- Make sure they're TMP InputFields, not regular InputFields
- Check they're assigned to UIManager

**"START QUIZ button doesn't appear"**
- You need to fill in BOTH name fields
- You need to select a school

**"Questions don't show"**
- Check Question Bank size is 30+
- Check first question is filled in completely
- Check Correct Answer Index is 0-3

**"Timer isn't ticking"**
- Check TimerLabel is assigned to QuizProper
- Check Time Per Question is set (default 20)

**"Score doesn't update"**
- This should happen automatically - check if you answered correctly
- Check ScoreLabel is assigned

---

## PHASE 10: VERIFY SAVING WORKS (2 minutes)

### Step 10.1: Complete a Full Quiz
1. Answer all 30 questions (quickly for testing)
2. After question 30, you should see a bonus applied
3. Click "Home" or "Restart"

### Step 10.2: Check the Save File
1. Stop playing
2. Open File Explorer
3. Navigate to: **C:\Users\[YourUsername]\AppData\LocalLow\DefaultCompany\[GameName]**
4. You should see a file called **player_scores.json**
5. Open it with Notepad
6. You should see your player record:
   ```json
   {
     "allRecords": [
       {
         "firstName": "Test",
         "lastName": "Player",
         "school": "TECH",
         "finalScore": 3250,
         "timestamp": "01/15/2026"
       }
     ]
   }
   ```

If you see this file, **everything is working!** ✓

---

## SUMMARY CHECKLIST

- [ ] **Phase 1**: Scene created
- [ ] **Phase 2**: 4 scripts created and pasted
- [ ] **Phase 3**: Instruction Panel UI created
- [ ] **Phase 4**: User Info Panel UI created
- [ ] **Phase 5**: Quiz UI created (buttons, timer, score, etc)
- [ ] **Phase 6**: GameObjects created and scripts attached
- [ ] **Phase 7**: All Inspector fields wired correctly
- [ ] **Phase 8**: 30+ questions added
- [ ] **Phase 9**: Tested and working
- [ ] **Phase 10**: Verified save file created
- [ ] **Phase 11**: Leaderboard scene created and wired
- [ ] **Phase 11**: Quiz connected to leaderboard
- [ ] **Phase 11**: Tested full flow (quiz → leaderboard)

---

## PHASE 11: CREATE LEADERBOARD SCENE (45 minutes)

Now let's create a leaderboard to display the 5 most recent quiz sessions!

### Step 11.1: Create Leaderboard Scene
1. Right-click in Project folder
2. Create > Scene
3. Name it **"Leaderboard"**
4. Save it in **Assets/Scenes/**
5. Double-click to open it

### Step 11.2: Create the Canvas
1. Right-click in Hierarchy
2. UI > Canvas
3. Set **Canvas Scaler** to "Scale With Screen Size"
4. Reference Resolution: 1080 x 1920 (portrait mode for mobile)
5. **ANDROID MOBILE**: Add **SafeAreaLayoutGroup** component (prevents UI cutoff on notched phones)

### Step 11.3: Create Background Panel
1. Right-click on Canvas
2. UI > Panel
3. Name: **BackgroundPanel**
4. Set Anchors to "Stretch" (full screen)
5. Customize color/image as desired

### Step 11.4: Create Title
1. Right-click on Canvas
2. UI > Text - TextMeshPro
3. Name: **TitleText**
4. Text: "LEADERBOARD"
5. Font Size: 80
6. Position at top center
7. Make it bold and stylish!

### Step 11.5: Create Current Player Panel (Optional but Recommended)
1. Right-click on Canvas
2. UI > Panel
3. Name: **CurrentPlayerPanel**
4. Position near top (below title)
5. Make it stand out (different color, border, etc.)

Inside CurrentPlayerPanel, create 4 text fields:

**Current Player Name:**
1. Right-click CurrentPlayerPanel
2. UI > Text - TextMeshPro
3. Name: **CurrentPlayerNameText**
4. Text: "Player Name"
5. Position: Top-left of panel

**Current Player School:**
1. UI > Text - TextMeshPro
2. Name: **CurrentPlayerSchoolText**
3. Text: "SCHOOL"
4. Position: Top-right of panel

**Current Player Date:**
1. UI > Text - TextMeshPro
2. Name: **CurrentPlayerDateText**
3. Text: "01/15/2026"
4. Position: Bottom-left of panel

**Current Player Score:**
1. UI > Text - TextMeshPro
2. Name: **CurrentPlayerScoreText**
3. Text: "0"
4. Position: Bottom-right of panel
5. Make it LARGE and highlighted (this is the player's achievement!)

### Step 11.6: Create Leaderboard Entries Container
1. Right-click on Canvas
2. Create Empty
3. Name: **LeaderboardEntriesContainer**
4. Position in the middle area
5. Add **Vertical Layout Group** component:
   - Spacing: 20
   - Child Alignment: Upper Center

### Step 11.7: Create 5 Leaderboard Entry Rows

You'll create 5 identical rows. Each row shows: Name | School | Date | Score

**For EACH of the 5 entries (Entry 1 through Entry 5):**

1. Right-click LeaderboardEntriesContainer
2. UI > Panel
3. Name: **LeaderboardEntry1** (then Entry2, Entry3, Entry4, Entry5)
4. Height: 150 units
5. Add **Horizontal Layout Group** component:
   - Spacing: 20
   - Child Force Expand: Width ✓, Height ✓

**Inside each Entry panel, create 4 text fields:**

**Name Text:**
1. UI > Text - TextMeshPro
2. Name: **NameText1** (NameText2, NameText3, etc.)
3. Text: "Player Name"
4. Alignment: Middle Left
5. Font Size: 40

**School Text:**
1. UI > Text - TextMeshPro
2. Name: **SchoolText1** (SchoolText2, etc.)
3. Text: "SCHOOL"
4. Alignment: Middle Center
5. Font Size: 35

**Date Text:**
1. UI > Text - TextMeshPro
2. Name: **DateText1** (DateText2, etc.)
3. Text: "01/15/2026"
4. Alignment: Middle Center
5. Font Size: 30

**Score Text:**
1. UI > Text - TextMeshPro
2. Name: **ScoreText1** (ScoreText2, etc.)
3. Text: "0"
4. Alignment: Middle Right
5. Font Size: 50
6. Color: Gold or bright color

**Repeat this for all 5 entries!** It's tedious but worth it. You can also duplicate the first entry and rename.

### Step 11.8: Create Empty State Text
1. Right-click on Canvas
2. UI > Text - TextMeshPro
3. Name: **EmptyStateText**
4. Text: "No quiz sessions yet. Play to create history!"
5. Font Size: 50
6. Alignment: Center
7. Position in the middle
8. **UNCHECK "Active"** (only shows when no records exist)

### Step 11.9: Create Navigation Buttons

**Back to Menu Button:**
1. Right-click on Canvas
2. UI > Button - TextMeshPro
3. Name: **BackToMenuButton**
4. Text: "MAIN MENU"
5. Position: Bottom-left corner
6. Size: 300 x 100

**Play Again Button:**
1. Right-click on Canvas
2. UI > Button - TextMeshPro
3. Name: **PlayAgainButton**
4. Text: "PLAY AGAIN"
5. Position: Bottom-right corner
6. Size: 300 x 100

### Step 11.10: Add LocalLeaderboardManager Script

**Important:** The script already exists in your project!

1. Right-click in Hierarchy (not on Canvas)
2. Create Empty
3. Name: **LocalLeaderboardManager**
4. Add Component > **LocalLeaderboardManager** (the script)

### Step 11.11: Wire Up LocalLeaderboardManager Inspector

Select the **LocalLeaderboardManager** GameObject:

**Leaderboard Display Section:**
- **Name Texts** - Set Size to 5:
  - Element 0 → Drag **NameText1**
  - Element 1 → Drag **NameText2**
  - Element 2 → Drag **NameText3**
  - Element 3 → Drag **NameText4**
  - Element 4 → Drag **NameText5**

- **School Texts** - Set Size to 5:
  - Element 0 → Drag **SchoolText1**
  - Element 1 → Drag **SchoolText2**
  - Element 2 → Drag **SchoolText3**
  - Element 3 → Drag **SchoolText4**
  - Element 4 → Drag **SchoolText5**

- **Date Texts** - Set Size to 5:
  - Element 0 → Drag **DateText1**
  - Element 1 → Drag **DateText2**
  - Element 2 → Drag **DateText3**
  - Element 3 → Drag **DateText4**
  - Element 4 → Drag **DateText5**

- **Score Texts** - Set Size to 5:
  - Element 0 → Drag **ScoreText1**
  - Element 1 → Drag **ScoreText2**
  - Element 2 → Drag **ScoreText3**
  - Element 3 → Drag **ScoreText4**
  - Element 4 → Drag **ScoreText5**

**Optional: Current Player Highlight Section:**
- Current Player Panel → Drag **CurrentPlayerPanel**
- Current Player Name Text → Drag **CurrentPlayerNameText**
- Current Player School Text → Drag **CurrentPlayerSchoolText**
- Current Player Date Text → Drag **CurrentPlayerDateText**
- Current Player Score Text → Drag **CurrentPlayerScoreText**

**Empty State Text:**
- Empty State Text → Drag **EmptyStateText**
- Empty Message → Leave as default: "No quiz sessions yet. Play to create history!"

**Navigation Buttons:**
- Back To Menu Button → Drag **BackToMenuButton**
- Play Again Button → Drag **PlayAgainButton**

**Scene Names:**
- Main Menu Scene Name → Type: "MainMenu"
- Quiz Scene Name → Type: "Quiz Scene Proper"

### Step 11.12: Add Leaderboard Scene to Build Settings

**CRITICAL STEP** - Without this, scene transitions won't work!

1. Click **File > Build Settings**
2. Click **Add Open Scenes** (with Leaderboard scene open)
3. Make sure these scenes are in order:
   - Scene 0: MainMenu
   - Scene 1: Quiz Scene Proper
   - Scene 2: Leaderboard
4. Click **Close**

### Step 11.13: Connect Quiz Scene to Leaderboard

Now we need to make the quiz go to the leaderboard when finished.

1. Open **Quiz Scene Proper** scene
2. Select **QuizProper** GameObject
3. In Inspector, find **"Scene To Load After Quiz"** field
4. Type: **"Leaderboard"**

### Step 11.14: Test Leaderboard with Debug Helper (Optional but Recommended)

Want to test without playing the full quiz? Use the debug helper!

1. In Leaderboard scene, right-click in Hierarchy
2. Create Empty
3. Name: **LeaderboardDebugHelper**
4. Add Component > **LeaderboardDebugHelper** (the script)

Now in the Inspector, you'll see helpful buttons:
- **Generate Test Record** - Creates 1 fake player
- **Generate Five Test Records** - Creates 5 fake players
- **Clear All Records** - Deletes everything
- **Refresh Leaderboard Display** - Updates the display

Click "Generate Five Test Records" to populate your leaderboard instantly!

### Step 11.15: Test Full Flow

1. **Play** from **MainMenu** scene
2. Complete the onboarding (name + school)
3. Start quiz
4. Answer questions (use lifelines, test features)
5. After 30 questions → Should automatically go to **Leaderboard**
6. You should see:
   - Your current session highlighted at top (if CurrentPlayerPanel exists)
   - Up to 5 most recent sessions below
   - Your score displayed
7. Click **PLAY AGAIN** → Goes back to quiz
8. Click **MAIN MENU** → Goes back to main menu

---

## What Happens Next?

After completing Phase 11, you now have:
1. ✅ Full quiz system with onboarding
2. ✅ 30 questions with lifelines and scoring
3. ✅ Local score saving
4. ✅ **Leaderboard showing 5 most recent sessions**

Next steps you can take:
- Connect to **LootLocker** for online leaderboards (global rankings!)
- Add **achievements** or **badges** for milestones
- Add **sound effects** and music
- Add **animations** for smoother transitions
- Create a **results screen** before the leaderboard (detailed breakdown)

But for now, you have a complete local quiz system with leaderboard! 🎉

---

## ANDROID MOBILE BUILD

**Important:** This game is built for Android mobile!

Once you've tested everything on PC (Play Mode), see **ANDROID_BUILD_GUIDE.md** for:
- Back button handling (already in code ✅)
- Safe area setup for notched phones
- Building and testing on Android device
- Performance optimization tips

Key Android changes already made:
- ✅ Back button (Escape key) pauses quiz
- ✅ No mouse-specific inputs (all touch-compatible)
- ✅ Proper coroutine management (no FPS drops)

---

## Questions?

Refer to:
- **SETUP_GUIDE.md** for script explanations
- **BUTTON_SHUFFLING_EXPLAINED.md** for answer shuffling details
- **HOW_TO_ADD_QUESTIONS.md** for adding questions
- **ANDROID_BUILD_GUIDE.md** for mobile build & deployment

Good luck, developer! You've got this! 💪
