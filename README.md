# Tammy-s-Quiz-Rush-SDG-Quest
FTIC Game Project

## Complete Leaderboard Setup Guide

### Overview
The local leaderboard system tracks the 5 most recent quiz sessions played on the device. It displays:
- Player name (First + Last name)
- School
- Date (MM/DD/YYYY)
- Score

**Note:** The leaderboard is accessible from the Main Menu scene. After completing a quiz, a results panel will show the player's performance feedback.

---

## PART 1: Quiz Results Panel Setup (DO THIS FIRST)

### Quiz Results System

After completing the quiz, a results panel fades in showing:
- **Performance Feedback** based on score:
  - **Below 1000**: "Need more improvement"
  - **1000 - 2500**: "Good"
  - **Above 2500**: "Excellent"
- **Final Score**

### Step-by-Step Results Panel Setup

#### Step 1: Open Quiz Scene
1. Open the "Quiz Scene Proper" scene in your Unity project

#### Step 2: Create Results Panel in Canvas
1. In the Hierarchy, find your Canvas (or create one if it doesn't exist)
2. Right-click on Canvas → UI → Panel → Panel
3. Rename it to "ResultsPanel"
4. Position and size it as desired (usually centered on screen)

#### Step 3: Add CanvasGroup Component
1. Select the ResultsPanel GameObject
2. In the Inspector, click "Add Component"
3. Search for and add "CanvasGroup"
4. Keep all settings at default (this enables fade-in effect)

#### Step 4: Create Text Elements Inside Panel
1. Select ResultsPanel in Hierarchy
2. Right-click → UI → Text - TextMeshPro
3. Rename to "ResultsMessageText"
4. Position near the top of the panel
5. Set font size to 48
6. Repeat for a second text:
   - Right-click ResultsPanel → UI → Text - TextMeshPro
   - Rename to "FinalScoreText"
   - Position below the message text
   - Set font size to 36

#### Step 5: Configure the Panel
1. Select ResultsPanel
2. In the Inspector, find the "Canvas Group" component
3. Set Alpha to 0 (it will fade in automatically)
4. In the Hierarchy, uncheck the checkbox next to ResultsPanel to set it INACTIVE

#### Step 6: Assign to QuizProper Script
1. Select the GameObject that has the QuizProper script
2. In the Inspector, find the "Results Panel" section
3. Drag ResultsPanel from Hierarchy to "Results Panel" field
4. Drag the ResultsPanel's CanvasGroup component to "Results Panel Canvas Group" field
5. Drag ResultsMessageText to "Results Message Text" field
6. Drag FinalScoreText to "Final Score Text" field

#### Step 7: Optional - Customize Messages
1. In QuizProper Inspector, under "Performance Feedback Messages":
   - Excellent Message: "Excellent!" (or your custom text)
   - Good Message: "Good!" (or your custom text)
   - Needs Improvement Message: "Need more improvement"
2. Under "Score Thresholds":
   - Excellent Threshold: 2500
   - Good Threshold: 1000

✅ **Results panel is now complete!**

---

## PART 2: Leaderboard Scene Setup (DO THIS SECOND)

### Step 1: Create New Leaderboard Scene
1. In Unity, go to File → New Scene
2. Go to File → Save Scene As
3. Save in: `Assets/Scenes/Leaderboard.unity`
4. Name it: "Leaderboard"

### Step 2: Create Canvas
1. In the empty scene, right-click in Hierarchy → UI → Canvas
2. Rename to "LeaderboardCanvas"

### Step 3: Add Title Text
1. Right-click LeaderboardCanvas → UI → Text - TextMeshPro
2. Rename to "TitleText"
3. In the Inspector:
   - Text: "Recent Quiz Sessions"
   - Font Size: 60
   - Anchor: Top Center
   - Position Y: -50

### Step 4: Create Header Row (Optional but Recommended)
1. Right-click LeaderboardCanvas → Create Empty
2. Rename to "HeaderRow"
3. Right-click HeaderRow → UI → Text - TextMeshPro
4. Create 4 texts in HeaderRow with names:
   - "NameHeader" - Text: "Name"
   - "SchoolHeader" - Text: "School"
   - "DateHeader" - Text: "Date"
   - "ScoreHeader" - Text: "Score"
5. Position them horizontally across the screen

### Step 5: Create Record Rows (The 5 Leaderboard Entries)

**For Record 1:**
1. Right-click LeaderboardCanvas → Create Empty
2. Rename to "Record1"
3. Create 4 TextMeshPro texts inside Record1:
   - "Name1" - For player name
   - "School1" - For school name
   - "Date1" - For date
   - "Score1" - For score
4. Position horizontally in a row

**For Records 2-5:**
- Repeat the same process for Record2, Record3, Record4, Record5
- Position each row below the previous one
- Recommended spacing: 15-20 pixels between rows

### Step 6: Create Empty State Text
1. Right-click LeaderboardCanvas → UI → Text - TextMeshPro
2. Rename to "EmptyStateText"
3. Text: "No quiz sessions yet. Play the quiz to create history!"
4. Font Size: 32
5. Position: Center of screen

### Step 7: Create Current Player Panel (Optional - for highlighting current player)
1. Right-click LeaderboardCanvas → UI → Panel
2. Rename to "CurrentPlayerPanel"
3. Position at top of screen
4. Add a background color to distinguish it
5. Inside the panel, create 4 TextMeshPro texts:
   - "CurrentPlayerNameText"
   - "CurrentPlayerSchoolText"
   - "CurrentPlayerDateText"
   - "CurrentPlayerScoreText"

### Step 8: Create Navigation Buttons
1. Right-click LeaderboardCanvas → UI → Button - TextMeshPro
2. Rename to "BackToMenuButton"
3. Text: "Back to Menu"
4. Position: Bottom left
5. Repeat for "PlayAgainButton"
   - Text: "Play Again"
   - Position: Bottom right

### Step 9: Create LeaderboardManager GameObject
1. Right-click LeaderboardCanvas → Create Empty
2. Rename to "LeaderboardManager"
3. Position doesn't matter (it's just for the script)

### Step 10: Attach LocalLeaderboardManager Script
1. Select LeaderboardManager GameObject
2. In Inspector, click "Add Component"
3. Search for and select "LocalLeaderboardManager"

### Step 11: Assign All References in Inspector

**Array Assignments (All have Size: 5):**

**Name Texts:**
1. Click the array size field, set to 5
2. Element 0: Drag Name1 from Hierarchy
3. Element 1: Drag Name2
4. Element 2: Drag Name3
5. Element 3: Drag Name4
6. Element 4: Drag Name5

**School Texts:**
1. Click the array size field, set to 5
2. Element 0: Drag School1 from Hierarchy
3. Element 1: Drag School2
4. Element 2: Drag School3
5. Element 3: Drag School4
6. Element 4: Drag School5

**Date Texts:**
1. Click the array size field, set to 5
2. Element 0: Drag Date1 from Hierarchy
3. Element 1: Drag Date2
4. Element 2: Drag Date3
5. Element 3: Drag Date4
6. Element 4: Drag Date5

**Score Texts:**
1. Click the array size field, set to 5
2. Element 0: Drag Score1 from Hierarchy
3. Element 1: Drag Score2
4. Element 2: Drag Score3
5. Element 3: Drag Score4
6. Element 4: Drag Score5

**Single Element Assignments:**

1. **Current Player Panel**: Drag CurrentPlayerPanel
2. **Current Player Name Text**: Drag CurrentPlayerNameText
3. **Current Player School Text**: Drag CurrentPlayerSchoolText
4. **Current Player Date Text**: Drag CurrentPlayerDateText
5. **Current Player Score Text**: Drag CurrentPlayerScoreText
6. **Empty State Text**: Drag EmptyStateText
7. **Back To Menu Button**: Drag BackToMenuButton
8. **Play Again Button**: Drag PlayAgainButton

**Scene Name Assignments:**
1. Main Menu Scene Name: "MainMenu"
2. Quiz Scene Name: "Quiz Scene Proper"

### Step 12: Add Scene to Build Settings
1. Go to File → Build Settings
2. Drag the Leaderboard scene from Project folder into the "Scenes In Build" list
3. Note the scene index number (usually 3 or 4)

✅ **Leaderboard scene is now complete!**

---

## PART 3: Add Leaderboard Button to Main Menu

### Step 1: Open Main Menu Scene
1. Open your "MainMenu" scene

### Step 2: Create Leaderboard Button
1. In the Canvas, right-click → UI → Button - TextMeshPro
2. Rename to "ViewLeaderboardButton"
3. Set text to "View Leaderboard"
4. Position it somewhere on your menu

### Step 3: Add OnClick Listener
1. Select ViewLeaderboardButton
2. In Inspector, find the Button component
3. Click the "+" under OnClick()
4. Drag the ViewLeaderboardButton into the object field
5. From the dropdown, select SceneLoader (or create a simple script)
6. Select the function that loads scenes

### Step 4: Simple Alternative - Use Code
1. Select ViewLeaderboardButton
2. Add Component → New Script
3. Name it "LeaderboardButtonHandler"
4. Add this code:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardButtonHandler : MonoBehaviour
{
    public void ViewLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }
}
```

5. In Button's OnClick(), drag the GameObject to object field
6. Select LeaderboardButtonHandler → ViewLeaderboard()

✅ **Leaderboard menu button is now complete!**

---

## Testing Your Setup

### Generate Test Data
1. Create an empty GameObject in any scene
2. Add Component → New Script → LeaderboardDebugHelper
3. In Inspector, click buttons to generate test records:
   - "Generate 1 Test Record" - Creates one random entry
   - "Generate 5 Test Records" - Creates 5 entries
   - "View Recent 5" - Shows all entries in console
   - "Clear All Records" - Deletes all data (for testing)

### Test the Flow
1. Play Quiz Scene
2. Complete the quiz
3. See results panel fade in
4. Go to Main Menu
5. Click "View Leaderboard" button
6. See the 5 most recent entries
7. Test "Back to Menu" and "Play Again" buttons

---

## Script Files Reference

- **LocalLeaderboardManager.cs** - Displays leaderboard data
- **PlayerManager.cs** - Saves and loads player records
- **QuizProper.cs** - Shows results panel after quiz
- **LeaderboardDebugHelper.cs** - Testing and debugging tools
- **LeaderboardUISetupGuide.cs** - UI structure documentation

---

## Data Storage

Player records are saved locally in JSON format at:
`Application.persistentDataPath/player_scores.json`

On different platforms:
- **Windows**: `C:\Users\[Username]\AppData\LocalLow\[Company]\[Product]\`
- **Mac**: `~/Library/Application Support/[Company]/[Product]/`
- **Android**: `/data/data/[package.name]/files/`

---

## Troubleshooting

### Leaderboard shows "No records yet"
- Make sure you've played the quiz at least once
- Check Debug Console for save errors
- Use LeaderboardDebugHelper to generate test data

### Results panel doesn't appear
- Make sure ResultsPanel is set to INACTIVE in Scene
- Verify CanvasGroup is assigned in QuizProper Inspector
- Check that Alpha is set to 0 on the CanvasGroup

### Text fields are empty
- Verify all TextMeshPro references are assigned
- Check that player data was saved (use Debug Console)
- Use LeaderboardDebugHelper to view saved records

### Buttons don't work
- Make sure scene names match exactly: "MainMenu", "Quiz Scene Proper", "Leaderboard"
- Verify Button component has OnClick listener assigned
- Check that scene is added to Build Settings
- Use the Inspector buttons to generate test records