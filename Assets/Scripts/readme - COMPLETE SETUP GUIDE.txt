TAMMY'S QUIZ RUSH SDG QUEST - COMPLETE SETUP GUIDE
==================================================

Purpose
-------
This guide is for the next developer who will maintain, customize, and deploy this Unity project.
It covers:
- Full script overview and ownership
- How to modify gameplay values in Inspector
- Question bank customization
- Transition setup
- Scene flow and sequence
- Local leaderboard, global leaderboard, and school ranking systems
- Common change requests and where to edit

Project at a Glance
-------------------
Core game behavior is implemented mainly in:
1) Assets/Scripts/Quiz Proper/QuizProper.cs
2) Assets/Scripts/Quiz Proper/UIManager.cs
3) Assets/Scripts/Quiz Proper/QuestionData.cs
4) Assets/Scripts/Quiz Proper/PlayerManager.cs

Supporting systems:
- Scene and loading scripts under Assets/Scripts/WorkingFiles/
- Leaderboard screens under Assets/Scripts/Leaderboard/
- Cutscene script under Assets/Scripts/Cutscenes/
- Transition runtime under Assets/EasyTransitions/Scripts/

Important note on leaderboard ownership and credentials
------------------------------------------------------
Global and school ranking boards currently use Unity Gaming Services (UGS) Leaderboards, referenced by leaderboardId (default FTICSDGQuest) inside:
- PlayerManager.cs
- GlobalLeaderboardDisplay.cs
- SchoolLeaderboard.cs

As discussed by the team, this leaderboard resource is currently tied to one intern's UGS setup/account.
If you need to change environment, dashboard project, credentials, service account ownership, or data governance:
- Coordinate with and contact the intern / previous intern in charge first.
- Migrate IDs and service config intentionally to avoid losing production score history.


=================================================================
A) SCRIPT REFERENCE (ALL GAME-OWNED SCRIPTS)
=================================================================

A1) Quiz Core Scripts
---------------------
1. Assets/Scripts/Quiz Proper/QuizProper.cs
   - Main quiz flow: instructions, user-info handoff, question loading, timer, answer checks, score logic,
     lifelines, pause/resume, result panel, ending scene routing.
   - Most gameplay tuning is done in this script's SerializeField Inspector values.

2. Assets/Scripts/Quiz Proper/UIManager.cs
   - User info panel behavior: first name, last name, school selection (including OTHERS abbreviation),
     form validation, and Start Quiz handoff to QuizProper + PlayerManager session setup.

3. Assets/Scripts/Quiz Proper/QuestionData.cs
   - Data classes for question format:
     - AnswerChoice (text/image mode)
     - QuizQuestion (question text, 4 answers, correct index, feedback messages)
   - Used by QuizProper.questionBank.

4. Assets/Scripts/Quiz Proper/PlayerManager.cs
   - Persistent singleton (DontDestroyOnLoad).
   - Saves local records to JSON (player_scores.json in Application.persistentDataPath).
   - Handles UGS service initialization/sign-in and score upload with metadata.

A2) Leaderboard Scripts
-----------------------
5. Assets/Scripts/Leaderboard/LocalLeaderboardManager.cs
   - Displays top 5 local records (from JSON via PlayerManager).
   - Handles back-to-menu, play-again, refresh, clear-records buttons.

6. Assets/Scripts/Leaderboard/LocalLeaderboardManagerEditor.cs
   - Custom editor helper for LocalLeaderboardManager debug actions in Unity Editor.

7. Assets/Scripts/Leaderboard/GlobalLeaderboardDisplay.cs
   - Fetches top 5 global entries from UGS Leaderboards with metadata (name + school).

8. Assets/Scripts/Leaderboard/SchoolLeaderboard.cs
   - Fetches global entries from UGS then filters into:
     FEU-TECH, FEU-DILIMAN, FEU-ALABANG, and OTHERS panels.

9. Assets/Scripts/Leaderboard/GlobalLeaderBoardManager.cs
   - Alternate/legacy global board integration using Dan.Main.LeaderboardCreator plugin (public key based).
   - Not the same backend as UGS.

10. Assets/Scripts/Leaderboard/GlobalScoreManager.cs
    - Simple submit helper for the Dan.Main.LeaderboardCreator flow.

A3) Cutscene / Transition / Scene Utility Scripts
--------------------------------------------------
11. Assets/Scripts/Cutscenes/Cutscene Script.cs (class CutsceneScript)
    - Plays intro video, enables delayed skip, and loads quiz scene with EasyTransition.

12. Assets/Scripts/Cutscene Manager/CutsceneManager.cs
    - Fully commented-out legacy version; no runtime effect unless revived.

13. Assets/Scripts/WorkingFiles/TestTransitionLoader.cs
    - Utility scene loader using TransitionManager + TransitionSettings.

14. Assets/Scripts/WorkingFiles/SceneLoader.cs
    - Generic LoadScene by name/index utility for button OnClick hooks.

15. Assets/Scripts/WorkingFiles/Loading/WarningLabel.cs (class WarningTextController)
    - Internet warning display/fade then scene load.

16. Assets/Scripts/WorkingFiles/Loading/LoadingScene.cs (class VideoLoadingScreen)
    - Plays loading video while preloading next scene asynchronously.

A4) Main Menu / Misc Interaction Scripts
-----------------------------------------
17. Assets/Scripts/WorkingFiles/MainMenu/TogglemusicBehavior.cs (class AudioToggleManager)
    - Music/SFX toggles, PlayerPrefs persistence, optional UI click SFX routing.

18. Assets/Scripts/WorkingFiles/MainMenu/Holographic.cs (class HolographicUI)
    - Visual holographic flicker/pulse effect for layered UI images.

19. Assets/Scripts/WorkingFiles/AnimsTest.cs (class ToggleAnimatedButton)
    - Alternates Animator triggers Move1 / Move2 on each button press.

20. Assets/Scripts/WorkingFiles/LearnModeController.cs
    - Learn mode topic selection and next/prev navigation for title/content/icon arrays.

A5) External Runtime Dependencies Used by Game Flow
----------------------------------------------------
21. Assets/EasyTransitions/Scripts/TransitionSettings.cs
22. Assets/EasyTransitions/Scripts/TransitionManager.cs
23. Assets/EasyTransitions/Scripts/Transition.cs
24. Assets/EasyTransitions/Scripts/CutoutMaskUI.cs

25. Assets/UpdatedLeaderboards/LeaderboardCreator/Scripts/Main/LeaderboardCreator.cs
    - External plugin used by legacy GlobalLeaderBoardManager path.


=================================================================
B) CORE SETUP STEPS FOR A NEW DEVELOPER
=================================================================

B1) Open and Verify Build Scenes
--------------------------------
Current enabled order (ProjectSettings/EditorBuildSettings.asset):
1. Assets/Scenes/Loading/Warning 1.unity
2. Assets/Scenes/Cutscene_Before_Quiz.unity
3. Assets/Scenes/Loading/Loading1.unity
4. Assets/Scenes/MainMenu 1.unity
5. Assets/Scenes/Learn.unity
6. Assets/Scenes/Leaderboard 1.unity
7. Assets/Scenes/Quiz Scene Proper 1.unity
8. Assets/Scenes/Ending/GoodEnding.unity
9. Assets/Scenes/Ending/BadEnding.unity

Action:
- Ensure these scene assets still exist and names match all scene string fields in scripts.
- If scene names changed (for example MainMenu vs MainMenu 1), update inspector string fields accordingly.

B2) Main Wiring Checklist
-------------------------
In each scene, confirm references are assigned in Inspector:
- Quiz scene object with QuizProper:
  questionLabel, timerLabel, scoreLabel, questionCounterLabel
  answerButtons[4], answerTexts[4], answerImages[4]
  nextQuestionButton, pause/resume/restart/home buttons, panels
  lifeline buttons + lifeline gained panel refs
  instruction panels + instruction next buttons
  results panel refs + results next button
  uiManager reference

- UI object with UIManager:
  userInfoPanel, name inputs, school buttons, start button, quizPanel, quizProper ref

- Scene containing PlayerManager singleton prefab/object exists before quiz/leaderboard access

- Leaderboard scenes:
  arrays in LocalLeaderboardManager / GlobalLeaderboardDisplay / SchoolLeaderboard all assigned (size 5)


=================================================================
C) HOW TO CUSTOMIZE GAMEPLAY (INSPECTOR + DATA)
=================================================================

C1) Question Bank (add/edit/remove questions)
---------------------------------------------
Where:
- Quiz scene object with QuizProper.cs
- Field: questionBank (List<QuizQuestion>)

Each QuizQuestion requires:
- questionText
- answers (exactly 4 entries recommended by current logic)
- correctAnswerIndex (0..3)
- per-question feedback strings:
  correctFeedback, wrongFeedback, timeUpFeedback

Each AnswerChoice entry:
- answerText
- answerImage
- useImage (true=image mode, false=text mode)

Important behavior assumptions:
- QuizProper validates that answers.Count >= 4.
- Correct index is mapped through shuffled button positions, so do not rely on static button order.

C2) Number of Questions Per Run
-------------------------------
Where:
- QuizProper -> totalQuestionsInGame

Behavior:
- At StartQuiz(), questionBank is shuffled and truncated via Take(totalQuestionsInGame).

Recommendations:
- Keep totalQuestionsInGame <= questionBank.Count.
- If you raise total question count, re-check score thresholds and timing for balance.

C3) Timer and Pacing
--------------------
Where:
- QuizProper -> timePerQuestion
- QuizProper -> nextButtonFadeDelay, nextButtonFadeDuration

Behavior:
- Timer decrements in TimerCountDown coroutine.
- Stop Time lifeline pauses the timer for current question.

C4) Scoring and Difficulty Balance
----------------------------------
Where:
- QuizProper -> pointsFastAnswer, pointsMediumAnswer, pointsSlowAnswer
- QuizProper -> lifelineBonus
- QuizProper -> excellentThreshold, goodThreshold
- QuizProper -> cutsceneScoreThreshold

Behavior:
- Fast/Medium/Slow based on answer time:
  <=5 sec, <=12 sec, >12 sec
- Double Points lifeline doubles next correct answer only.
- Unused lifelines add end bonus in FinishQuiz().
- Results message + ending scene depend on final score thresholds.

C5) Lifelines (granting and usage)
----------------------------------
Types:
- Stop Time
- Double Points
- Guided Questions (Reduce Choices)

Key logic in QuizProper:
- Streak trigger: correctAnswerStreak == 3 (ShouldTriggerStreakLifeline)
- Guaranteed checkpoint trigger: question number 10 (IsGuaranteedLifelineQuestion)
- Award only from unowned lifelines (GetAvailableLifelines)

Inspector tuning in QuizProper:
- lifelineRouletteDuration, lifelineRouletteInterval
- lifelineDisabledOpacity, reducedChoiceOpacity
- stopTimeSprite, doublePointsSprite, reduceChoicesSprite
- UI/animation fields for panel slide/fade and dim background

C6) Feedback Text and Button Animation Tuning
---------------------------------------------
Where:
- QuizProper animation and feedback groups

You can tune:
- Correct/wrong/timeup colors
- Blink speed/count
- Button scale durations
- Non-clicked fade settings
- Correct answer highlight values

C7) User Info Form Rules
------------------------
Where:
- UIManager.cs

Current rules:
- Start button shown only when first name + last name + school are valid.
- If OTHERS selected, abbreviation input becomes required.
- All values are normalized to uppercase before save.

To add fields (example: section, year level):
1. Add TMP_InputField and serialized reference in UIManager.
2. Extend ValidateForm().
3. Extend PlayerManager.SetPlayerInfo and metadata upload payload.
4. Extend leaderboard display if needed.


=================================================================
D) TRANSITIONS SETUP GUIDE (EASYTRANSITION)
=================================================================

D1) Required Runtime Objects
----------------------------
- One TransitionManager in active scene(s) that trigger transitions.
- A TransitionSettings asset assigned in scripts that call transitions.

Used by project scripts:
- CutsceneScript.LoadQuizScene() -> TransitionManager.Instance().Transition(quizSceneName, transition, startDelay)
- TestTransitionLoader.LoadScene()

D2) Create/Configure TransitionSettings Asset
----------------------------------------------
1. Create asset from Create menu:
   Florian Butz/New Transition Settings
2. Configure:
   - transitionIn and transitionOut prefabs
   - colorTintMode and colorTint
   - transitionSpeed
   - transitionTime and destroyTime
   - blockRaycasts
3. Assign this asset to:
   - CutsceneScript.transition
   - TestTransitionLoader.transition (if used)

D3) Common Transition Troubleshooting
-------------------------------------
- Error "You tried to access the instance before it exists": no TransitionManager in scene.
- Transition starts but no visual: transitionIn/transitionOut prefabs not assigned.
- Wrong cut timing: tune transitionTime and autoAdjustTransitionTime.


=================================================================
E) SCENES AND SEQUENTIAL FLOW
=================================================================

E1) Typical Runtime Flow
------------------------
1. Warning scene (WarningTextController) checks internet status and loads next scene.
2. Intro cutscene scene (CutsceneScript) plays video, skip appears after delay, then transitions.
3. Loading video scene (VideoLoadingScreen) preloads target scene while video plays.
4. Main menu scene uses button loaders (SceneLoader/TestTransitionLoader).
5. Quiz scene:
   - Instruction panel 1 -> Instruction panel 2 -> User info panel (UIManager)
   - QuizProper starts question loop
   - Results panel shows feedback and score
   - Next button routes to GoodEnding or BadEnding based on cutsceneScoreThreshold
6. Leaderboard scenes display local/global/school rankings.

E2) If You Change Scene Names
-----------------------------
Update string fields in these scripts/inspectors:
- QuizProper.homeSceneName
- QuizProper.goodCutsceneSceneName
- QuizProper.badCutsceneSceneName
- LocalLeaderboardManager.mainMenuSceneName
- LocalLeaderboardManager.quizSceneName
- GlobalLeaderboardDisplay.mainMenuSceneName
- SchoolLeaderboard.mainMenuSceneName
- WarningTextController.nextSceneName
- VideoLoadingScreen.sceneToLoad
- CutsceneScript.quizSceneName

Recommendation:
- Prefer assigning scene via SceneAsset where available (CutsceneScript supports this in Editor).


=================================================================
F) LEADERBOARD SYSTEMS - DETAILED
=================================================================

F1) Local Leaderboard (offline JSON)
------------------------------------
Source of truth:
- PlayerManager saves to player_scores.json in Application.persistentDataPath.

When writes happen:
- QuizProper.FinishQuiz() -> PlayerManager.SetFinalScore(currentScore) -> SavePlayerRecord()

Display:
- LocalLeaderboardManager.GetTopLocalScores(5)
- Shows name, school, date, score (top scores sorted descending)

Good for:
- Offline score history on same device
- Debug/testing without online services

F2) Global Leaderboard (UGS)
----------------------------
Upload path:
- PlayerManager.SubmitScoreToLeaderboardAsync(score)
- Metadata uploaded: name + school

Display path:
- GlobalLeaderboardDisplay calls LeaderboardsService.GetScoresAsync with IncludeMetadata

Requirements:
- Unity Services initialized
- Anonymous sign-in valid
- Correct leaderboardId

If changing UGS project/leaderboard:
1. Confirm new UGS project and leaderboard in dashboard.
2. Replace leaderboardId in Inspector (or in scripts if hardcoded).
3. Validate authentication and read/write permissions.
4. Coordinate with intern/previous intern who owns current service setup.

F3) School Rankings (UGS filtered)
----------------------------------
Implemented in SchoolLeaderboard.cs by downloading global scores then filtering metadata.school.

School buckets:
- FEU-TECH
- FEU-DILIMAN
- FEU-ALABANG
- OTHERS (anything not matching the three FEU codes)

If adding a new official school bucket:
1. Add new enum value in SchoolType.
2. Add UI button + panel + arrays.
3. Extend filter logic and GetSchoolName mapping.
4. Update scene prefab and inspector assignments.

F4) Legacy/Alternative Global Board Plugin
-------------------------------------------
Scripts:
- GlobalLeaderBoardManager.cs
- GlobalScoreManager.cs
- Dan.Main.LeaderboardCreator plugin

This is a separate backend from UGS.
If you continue standard project flow, prioritize UGS scripts above.


=================================================================
G) IF A DEVELOPER CHANGES SOMETHING (CHANGE GUIDE)
=================================================================

G1) "I changed question count and score feels off"
--------------------------------------------------
Rebalance these in QuizProper:
- pointsFastAnswer / pointsMediumAnswer / pointsSlowAnswer
- excellentThreshold / goodThreshold
- cutsceneScoreThreshold

G2) "I changed answer button layout and now correctness is weird"
-----------------------------------------------------------------
Check these requirements in QuizProper:
- answerButtons length must remain 4
- answerTexts length must remain 4
- answerImages length should map 1:1 with answers
- correctAnswerIndex must stay within 0..3

G3) "Leaderboard not showing scores"
-------------------------------------
Checklist:
1. PlayerManager exists in runtime and not duplicated unexpectedly.
2. leaderboardId matches existing UGS leaderboard.
3. Unity Services initialization and sign-in succeed.
4. Metadata parsing not broken (name/school fields).
5. Internet connectivity available for global/school boards.
6. For local board, verify player_scores.json exists and is not malformed.

G4) "Cutscene does not transition"
-----------------------------------
Check:
- TransitionManager object exists in scene.
- CutsceneScript.transition assigned.
- quizSceneName valid (or quizSceneAsset assigned in editor).
- Video event wiring (prepareCompleted and loopPointReached) intact.

G5) "Pause/menu behaves strangely"
-----------------------------------
Pause uses Time.timeScale = 0 and resume restores to 1.
If adding new animations/UI during pause, ensure they use unscaled time when needed.
QuizProper includes AnimatorUnscaledTime helper class for this scenario.


=================================================================
H) QUICK INSPECTOR AUDIT BEFORE RELEASE
=================================================================

1. QuizProper questionBank populated and valid.
2. totalQuestionsInGame <= questionBank.Count.
3. All button/text/image arrays assigned and size 4 where required.
4. Instruction panel references and next buttons assigned.
5. Results panel and result texts assigned.
6. UGS leaderboardId consistent across PlayerManager, GlobalLeaderboardDisplay, SchoolLeaderboard.
7. Scene name string fields updated to actual scene names in Build Settings.
8. TransitionManager present where transition calls happen.
9. Audio toggles and sources mapped in menu scene.
10. Leaderboard arrays have exactly 5 display references each.


End of Complete Setup Guide
