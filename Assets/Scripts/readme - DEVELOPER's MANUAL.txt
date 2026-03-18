TAMMY'S QUIZ RUSH SDG QUEST - DEVELOPER'S MANUAL
================================================

1) SHORT OVERVIEW
-----------------
Tammy's Quiz Rush SDG Quest is a Unity quiz game with a guided pre-quiz onboarding flow, timed multi-choice gameplay, streak/checkpoint-based lifeline rewards, and score-based ending branches. The architecture is centered around four main scripts: QuizProper.cs (quiz loop and game rules), UIManager.cs (player input form and validation), QuestionData.cs (question model), and PlayerManager.cs (local persistence + Unity Gaming Services leaderboard upload).

The game also includes local, global, and school-filtered leaderboard displays. Local ranking is stored as JSON on the device, while global and school rankings are retrieved from UGS Leaderboards using metadata. A legacy plugin-based leaderboard path still exists (Dan.Main LeaderboardCreator), but the primary online flow in this project is the UGS flow.

Game flow at runtime is scene-based: warning/internet notice -> intro cutscene + transition -> loading -> main menu -> quiz -> ending cutscene (good/bad by score) and leaderboard views. Transition effects use EasyTransition assets and TransitionManager runtime.


2) IMPORT PACKAGE GUIDE
-------------------------------
If converting this project into one or more Unity packages, expect the following grouped content:

A) Core Gameplay Package
- Assets/Scripts/Quiz Proper/QuizProper.cs
- Assets/Scripts/Quiz Proper/UIManager.cs
- Assets/Scripts/Quiz Proper/QuestionData.cs
- Assets/Scripts/Quiz Proper/PlayerManager.cs
- Required scene/prefab/canvas references used by those scripts

B) Leaderboard UI Package
- Assets/Scripts/Leaderboard/LocalLeaderboardManager.cs
- Assets/Scripts/Leaderboard/LocalLeaderboardManagerEditor.cs
- Assets/Scripts/Leaderboard/GlobalLeaderboardDisplay.cs
- Assets/Scripts/Leaderboard/SchoolLeaderboard.cs
- Optional legacy scripts:
  - Assets/Scripts/Leaderboard/GlobalLeaderBoardManager.cs
  - Assets/Scripts/Leaderboard/GlobalScoreManager.cs

C) Transition/Cutscene Package
- Assets/Scripts/Cutscenes/Cutscene Script.cs
- Assets/Scripts/WorkingFiles/TestTransitionLoader.cs
- Assets/EasyTransitions/Scripts/*

D) Utility/Menu Package
- Assets/Scripts/WorkingFiles/SceneLoader.cs
- Assets/Scripts/WorkingFiles/Loading/WarningLabel.cs
- Assets/Scripts/WorkingFiles/Loading/LoadingScene.cs
- Assets/Scripts/WorkingFiles/MainMenu/TogglemusicBehavior.cs
- Assets/Scripts/WorkingFiles/MainMenu/Holographic.cs
- Assets/Scripts/WorkingFiles/AnimsTest.cs
- Assets/Scripts/WorkingFiles/LearnModeController.cs

When exporting/importing, include dependent assets (UI prefabs, TMP assets, transition prefabs, videos, sprites, scene assets) because many serialized fields are scene reference based. Missing references after import are expected unless all linked assets are included.


3) PROJECT SETUP (DETAILED BUT SHORTER VERSION)
-----------------------------------------------
Step 1: Scene and Build Configuration
- Verify scenes in Build Settings are present and correctly ordered.
- Current enabled order (from EditorBuildSettings.asset):
  1. Assets/Scenes/Loading/Warning 1.unity
  2. Assets/Scenes/Cutscene_Before_Quiz.unity
  3. Assets/Scenes/Loading/Loading1.unity
  4. Assets/Scenes/MainMenu 1.unity
  5. Assets/Scenes/Learn.unity
  6. Assets/Scenes/Leaderboard 1.unity
  7. Assets/Scenes/Quiz Scene Proper 1.unity
  8. Assets/Scenes/Ending/GoodEnding.unity
  9. Assets/Scenes/Ending/BadEnding.unity

Step 2: Core Quiz Wiring
- In Quiz scene, assign all required QuizProper references:
  question/timer/score labels, 4 answer buttons + 4 text + 4 images, next/pause controls,
  lifeline buttons/panels, instruction panels/buttons, results panel, UIManager reference.
- Populate questionBank with valid QuizQuestion items (4 answers each, valid correct index).

Step 3: User Input and Session Setup
- UIManager must reference QuizProper and PlayerManager should be present as singleton at runtime.
- UIManager validates names + school before enabling start.
- Player session starts via BeginNewPlayerSessionAsync before gameplay.

Step 4: Scoring and Difficulty Calibration
- Tune these QuizProper inspector fields:
  totalQuestionsInGame, timePerQuestion,
  pointsFastAnswer, pointsMediumAnswer, pointsSlowAnswer,
  lifelineBonus, excellentThreshold, goodThreshold, cutsceneScoreThreshold.

Step 5: Leaderboards
- Local leaderboard requires PlayerManager JSON save to function.
- Global and school leaderboards require UGS initialization, sign-in, and valid leaderboardId.
- Metadata expected in UGS entries: name and school.

Important service ownership note:
UGS leaderboard data/config is currently tied to one intern's setup. If you need to migrate/change that backend, coordinate with the intern / previous intern in charge before replacing IDs or credentials.

Step 6: Transition and Cutscene Setup
- Ensure TransitionManager exists where transitions are triggered.
- Assign TransitionSettings assets to CutsceneScript/TestTransitionLoader.
- Verify quizSceneName or SceneAsset is correct.

Step 7: Audio and Menu polish
- Configure AudioToggleManager sources/toggles/buttons.
- HolographicUI and ToggleAnimatedButton are optional polish scripts.


4) SCENES
---------
The scene logic can be understood as an orchestrated sequence of script responsibilities.

A) Warning Scene
- Script: WarningTextController (WarningLabel.cs)
- Purpose: checks internet reachability, displays advisory text, fades, then loads next scene.
- Key fields: warningText, warningImage, displayTime, fadeDuration, nextSceneName, internetRequired.

B) Intro Cutscene Scene
- Script: CutsceneScript (Cutscene Script.cs)
- Purpose: prepares and plays intro video, enables skip after delay, then transitions to quiz scene.
- Key fields: videoPlayer, introVideo, audioSource, videoVolume, skipButton, skipDelay,
  quizSceneAsset/quizSceneName, transition, startDelay.

C) Loading Scene
- Script: VideoLoadingScreen (LoadingScene.cs)
- Purpose: plays loading video while asynchronously loading target scene.

D) Main Menu Scene
- Scripts commonly used:
  SceneLoader.cs or TestTransitionLoader.cs for scene navigation,
  AudioToggleManager (TogglemusicBehavior.cs) for music/SFX toggles,
  HolographicUI for UI effect, ToggleAnimatedButton for anim toggles.

E) Learn Scene
- Script: LearnModeController.cs
- Purpose: topic browser with selection buttons and next/prev navigation.

F) Quiz Scene
- Core scripts: QuizProper.cs + UIManager.cs + QuestionData.cs + PlayerManager.cs (singleton persists)
- Flow: Instruction 1 -> Instruction 2 -> User info -> Quiz loop -> Results panel -> Ending route.

G) Leaderboard Scene(s)
- Local leaderboard: LocalLeaderboardManager.cs (JSON-driven top local scores)
- Global leaderboard: GlobalLeaderboardDisplay.cs (UGS top global)
- School ranking: SchoolLeaderboard.cs (UGS filtered by school metadata)
- Legacy alternative path (if retained): GlobalLeaderBoardManager.cs + GlobalScoreManager.cs

H) Ending Scenes
- Routed from QuizProper based on cutsceneScoreThreshold:
  goodCutsceneSceneName if score >= threshold, else badCutsceneSceneName.


APPENDIX: SCRIPT INDEX REFERENCE
--------------------------------
Core gameplay:
- Assets/Scripts/Quiz Proper/QuizProper.cs
- Assets/Scripts/Quiz Proper/UIManager.cs
- Assets/Scripts/Quiz Proper/QuestionData.cs
- Assets/Scripts/Quiz Proper/PlayerManager.cs

Leaderboard:
- Assets/Scripts/Leaderboard/LocalLeaderboardManager.cs
- Assets/Scripts/Leaderboard/LocalLeaderboardManagerEditor.cs
- Assets/Scripts/Leaderboard/GlobalLeaderboardDisplay.cs
- Assets/Scripts/Leaderboard/SchoolLeaderboard.cs
- Assets/Scripts/Leaderboard/GlobalLeaderBoardManager.cs
- Assets/Scripts/Leaderboard/GlobalScoreManager.cs

Cutscene / transition / scene utility:
- Assets/Scripts/Cutscenes/Cutscene Script.cs
- Assets/Scripts/Cutscene Manager/CutsceneManager.cs (legacy, commented)
- Assets/Scripts/WorkingFiles/TestTransitionLoader.cs
- Assets/Scripts/WorkingFiles/SceneLoader.cs
- Assets/Scripts/WorkingFiles/Loading/WarningLabel.cs
- Assets/Scripts/WorkingFiles/Loading/LoadingScene.cs

Menu/misc:
- Assets/Scripts/WorkingFiles/MainMenu/TogglemusicBehavior.cs
- Assets/Scripts/WorkingFiles/MainMenu/Holographic.cs
- Assets/Scripts/WorkingFiles/AnimsTest.cs
- Assets/Scripts/WorkingFiles/LearnModeController.cs

External runtime dependencies used by project scripts:
- Assets/EasyTransitions/Scripts/TransitionSettings.cs
- Assets/EasyTransitions/Scripts/TransitionManager.cs
- Assets/EasyTransitions/Scripts/Transition.cs
- Assets/EasyTransitions/Scripts/CutoutMaskUI.cs
- Assets/UpdatedLeaderboards/LeaderboardCreator/Scripts/Main/LeaderboardCreator.cs
