# ANDROID MOBILE BUILD SETUP GUIDE

## Overview

Your quiz game is now optimized for Android mobile devices. This guide covers the essential Android-specific configurations.

---

## PART 1: VITAL ANDROID CHANGES (ALREADY IN CODE)

### Change #1: Back Button Handling ✅

**What was added:** Android back button (`Escape` key) support in `QuizProper.cs`

**Code added (in `Update()` method):**
```csharp
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
```

**Why this matters:**
- Android phones have a back button (either physical or in navigation bar)
- Without this, pressing back would exit your app
- Now pressing back pauses the quiz, letting players continue

**How to test on Android:**
1. Build to Android device
2. Start a quiz
3. Press the back button (bottom-left or bottom-center button)
4. Pause menu should appear
5. You can resume or go home

---

## PART 2: SAFE AREA FOR NOTCHED PHONES ⚠️ IMPORTANT

### What's a Safe Area?

Modern phones have:
- Notches (iPhone style)
- Punch holes (Samsung, Pixel)
- Curved edges

These can cut off your UI if not handled properly.

### How to Fix It (Manual Step - REQUIRED!)

**Step 1: Select Your Canvas**
1. In Hierarchy, click on **Canvas**
2. In Inspector, click **Add Component**
3. Search for **"SafeAreaLayoutGroup"**
4. Add it

**Step 2: Configure It**
1. You should now see these properties in Inspector:
   - Ignore Layout
   - Layout Element
2. Leave all defaults (they're correct)

**That's it!** The Canvas will automatically avoid notches and safe areas.

### Why This Matters

**WITHOUT Safe Area:**
- UI buttons can appear behind notch
- InputFields can be unreachable
- Text can be cut off

**WITH Safe Area:**
- Everything adjusts automatically
- Works on all Android phones

### Testing Safe Area

1. In Play mode, go to **Gizmos** menu (top right)
2. Check **Show Safe Area**
3. You'll see a yellow outline showing the safe area
4. All UI should fit inside this area

---

## PART 3: SCREEN ORIENTATION (RECOMMENDED)

### Lock to Portrait Mode

**Why?**
- Quiz apps are played in portrait (vertical)
- Rotating the phone disrupts the quiz

### How to Set It (One-Time Setup)

1. Go to **File > Build Settings**
2. At the bottom, click **Player Settings**
3. In the Inspector on the right:
   - Find **Resolution and Presentation**
   - Find **Orientation** section
   - Set **Default Orientation** to **Portrait Down**
4. Under **Allowed Orientations**, uncheck:
   - Portrait Reverse
   - Landscape Left
   - Landscape Right
   - (Keep only "Portrait" checked)

**Result:** Phone will only allow portrait orientation while playing your game.

---

## PART 4: BUILD TO ANDROID

### Prerequisites

You need:
- Android SDK (comes with Unity)
- Android JDK (comes with Unity)
- Actual Android device (or Android emulator)

### Build Steps

**Step 1: Set Build Target to Android**
1. **File > Build Settings**
2. In the **Platform** section (left side), click **Android**
3. Click **Switch Platform** (this may take 1-2 minutes)

**Step 2: Configure Build Settings**
1. Set **Scenes In Build**:
   - Click "Add Open Scenes"
   - Make sure your **QuizScene** is listed
2. Set **Company Name**: (e.g., "Your School Name")
3. Set **Product Name**: (e.g., "Quiz Rush")
4. Set **Bundle Identifier**: (e.g., "com.yourschool.quizrush")
   - Must be unique and use only lowercase letters and periods

**Step 3: Player Settings (Advanced)**

1. Go to **Edit > Project Settings > Player**
2. Under **Mobile:**
   - API Level: Set to **API 30+** (Android 11+)
   - CPU: Keep "ARM64" checked
   - Delete Unneeded Architectures: Check this

3. Under **Permissions:**
   - The game only needs **Internet** permission (uncheck others)

**Step 4: Build**

1. **File > Build and Run**
2. Connect Android device via USB
3. Enable USB Debugging on phone:
   - Go to **Settings > Developer Options**
   - Enable **USB Debugging**
   - Tap "Allow" when phone asks to trust the computer
4. Unity will build and automatically install on your phone

**Step 5: Test on Device**
1. App should launch automatically
2. Go through full quiz
3. Test back button during quiz (should pause)
4. Rotate phone (should stay in portrait)
5. Check that UI doesn't cut off

---

## PART 5: PERFORMANCE TIPS FOR MOBILE

### Problem: Game Runs Slow on Older Phones

**Solutions:**

**1. Reduce Question Images Size**
- Use smaller image files (max 512x512 pixels)
- Compress images (use TinyPNG.com)
- In Inspector: Set **Texture Compression** to "Normal"

**2. Disable Unused Features**
- If not using image answers, set all `answerImages` to null in Inspector
- This saves memory

**3. Limit Animations**
- Set **Next Button Fade Duration** to 0.2 instead of 0.4
- Reduces visual lag on slow devices

**4. Target Lower API Level**
- Go to **Player Settings > API Level**
- Can target API 28 instead of 30 if needed
- Larger device compatibility

---

## PART 6: TESTING CHECKLIST

Before submitting to Play Store:

- [ ] App launches and shows instruction panel
- [ ] Onboarding panel works (name input, school selection)
- [ ] Quiz starts and timer counts down
- [ ] Questions display with text and/or images
- [ ] Lifeline buttons work
- [ ] Answer buttons are responsive (no lag)
- [ ] Pause button works
- [ ] Back button pauses quiz (not exits app)
- [ ] Quiz finishes and shows final score
- [ ] Score is saved (check player_scores.json)
- [ ] App doesn't rotate (stays in portrait)
- [ ] UI doesn't get cut off on notched phones
- [ ] No crashes or errors

---

## PART 7: APK FILE

### Where Is My APK?

After building, the APK is located at:
```
C:\Users\YourUsername\AppData\Local\Temp\UnityBuild
```

Or in your project:
```
YourProject\Builds\Android\
```

### What's an APK?

It's an **Android application file** - like a .exe for Windows, but for Android phones.

### Can I Share It?

**Yes!** You can:
1. Email the APK to friends
2. They install it on their Android phone
3. They can play your game

**To install APK on phone:**
- Download the APK file to your phone
- Open file manager
- Tap the APK
- Phone will ask "Install from unknown source?" → Tap Yes
- App installs

---

## PART 8: TROUBLESHOOTING

### Problem: "App Crashes When Back Button Pressed"

**Solution:**
- Back button code is safe
- Check if pausePanel has all buttons assigned in Inspector
- Check Console for errors (Window > General > Console)

### Problem: "UI Gets Cut Off on My Phone"

**Solution:**
- Add SafeAreaLayoutGroup to Canvas (see Part 2)
- Check Show Safe Area in Gizmos
- Adjust UI positions to be inside safe area

### Problem: "Game Runs Slowly on Old Phone"

**Solution:**
- Reduce image sizes (Part 5)
- Lower target API Level
- Test on newer phone if available

### Problem: "Questions Don't Load"

**Solution:**
- Check Question Bank size is set to 30+
- Check first question is filled in completely
- Check Correct Answer Index is 0-3

---

## PART 9: NEXT STEPS

Once game is working on Android:

1. **Test thoroughly** on at least 2 different Android phones
2. **Get feedback** from players about difficulty and speed
3. **Consider adding:**
   - Sound effects
   - Leaderboard screen
   - Achievement system
   - Analytics (Google Firebase)

---

## QUICK REFERENCE: ANDROID BUILD CHECKLIST

- [ ] Canvas has **SafeAreaLayoutGroup** component
- [ ] **QuizProper.cs** has back button code (already added ✅)
- [ ] **Default Orientation** set to Portrait
- [ ] **API Level** set to 30+
- [ ] Questions are added (30+)
- [ ] Built to Android device
- [ ] Tested all features on actual phone
- [ ] No UI cutoff on notched phones
- [ ] Back button pauses quiz

---

**You're all set for Android!** 🎮📱
