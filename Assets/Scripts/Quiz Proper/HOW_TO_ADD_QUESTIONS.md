# HOW TO ADD QUESTIONS IN THE INSPECTOR

## What You're About to Do

You're going to add your 30 quiz questions directly in Unity's Inspector (the right side of the screen). No code needed - just typing!

---

## Step 1: Find the Question Bank

1. Click on your **QuizProper** GameObject in the Hierarchy
2. In the Inspector, find the **"Data"** section
3. You should see **"Question Bank"** with a size field

```
Question Bank
▼ (expand arrow)
Size: 0
```

---

## Step 2: Set the Size

1. Click on the **Size** field
2. Change it to **30** (or however many questions you have)
3. Press Enter

Now you should see:

```
Question Bank
▼ (expand arrow)
Size: 30
Element 0
Element 1
Element 2
...
Element 29
```

---

## Step 3: Expand and Fill First Question

1. Click the **▼ arrow** next to **Element 0** to expand it
2. You should see 4 fields:

```
Element 0
  ▼ Question Text (empty text box)
    Answers (empty list)
    Correct Answer Index (0)
    Feedback Explanation (empty text box)
```

---

## Step 4: Fill in Question Text

1. Click in the **Question Text** box
2. Type your question

**Example:**
```
What is the capital of France?
```

---

## Step 5: Add 4 Answer Choices

1. Find **Answers** and click the **▼** to expand it
2. Click **"Size"** and change it to **4**

Now you should see:

```
Answers
  Size: 4
  Element 0
  Element 1
  Element 2
  Element 3
```

---

## Step 6: Fill in Each Answer

For **Element 0**, expand it by clicking **▼**:

```
Element 0
  ▼ Answer Text: (type: "Paris")
    Answer Image: (keep empty - we're using text)
    Use Image: (checkbox - keep UNCHECKED)
```

**Repeat for Element 1, 2, 3 with your other answers:**

```
Element 1: "London"
Element 2: "Berlin"
Element 3: "Rome"
```

---

## Step 7: Set the Correct Answer Index

1. Find **Correct Answer Index**
2. Type the position of the correct answer (0, 1, 2, or 3)

**Example:**
```
Correct Answer Index: 0  (because "Paris" is Element 0)
```

---

## Step 8: Add Feedback Explanation

1. Click in **Feedback Explanation**
2. Type why this answer is correct

**Example:**
```
Paris is the capital and most populated city of France.
```

---

## Now Your First Question is Complete!

It should look like:

```
Element 0
  Question Text: "What is the capital of France?"
  Answers (Size: 4)
    Element 0: "Paris" (useImage: OFF)
    Element 1: "London" (useImage: OFF)
    Element 2: "Berlin" (useImage: OFF)
    Element 3: "Rome" (useImage: OFF)
  Correct Answer Index: 0
  Feedback Explanation: "Paris is the capital and most populated city of France."
```

---

## Step 9: Repeat for Questions 2-30

Do the same process for all remaining elements (1-29).

**Tips to go faster:**
- Collapse Element 0 by clicking **▼** (becomes **►**)
- Expand Element 1 and copy the same structure
- You'll get faster after the first few!

---

## Using Images Instead of Text (Optional)

If you want to show an **image** instead of text for an answer:

1. In **Element X**, find **Use Image**
2. **CHECK** the checkbox
3. An **Answer Image** field will appear
4. Drag your image into that field

**Example for a geography question:**
```
Element 0:
  Answer Text: (leave empty)
  Answer Image: (drag French flag here)
  Use Image: ✓ (CHECKED)
```

---

## Complete Example - 3 Questions

Here's what 3 complete questions would look like:

### Question 1
```
Question Text: "What is 2 + 2?"
Answers (Size 4):
  [0] "3" (text)
  [1] "4" (text) ← CORRECT
  [2] "5" (text)
  [3] "6" (text)
Correct Answer Index: 1
Feedback: "2 plus 2 equals 4"
```

### Question 2
```
Question Text: "What is the largest planet?"
Answers (Size 4):
  [0] "Venus" (text)
  [1] "Mars" (text)
  [2] "Jupiter" (text) ← CORRECT
  [3] "Saturn" (text)
Correct Answer Index: 2
Feedback: "Jupiter is the largest planet in our solar system"
```

### Question 3
```
Question Text: "Which is a mammal?"
Answers (Size 4):
  [0] "Fish" (image)
  [1] "Dog" (image) ← CORRECT
  [2] "Bird" (image)
  [3] "Snake" (image)
Correct Answer Index: 1
Feedback: "Dogs are mammals and care for their young"
```

---

## Common Mistakes to Avoid

### ❌ Mistake 1: Wrong Correct Answer Index
```
Answers:
  [0] "A"
  [1] "B" ← Correct answer
  [2] "C"
  [3] "D"

Correct Answer Index: 0  ❌ WRONG
Correct Answer Index: 1  ✓ RIGHT
```

### ❌ Mistake 2: Answers Size is 0
Make sure you set **Answers Size to 4**! If it's 0, there are no answers.

### ❌ Mistake 3: Forgetting the Feedback Explanation
If this is empty, the feedback will say:
```
"Correct! The answer is B.
Because ."  ← Weird sentence
```

Always fill in the explanation!

### ❌ Mistake 4: Mixing Text and Image
If you check "Use Image" but don't drag an image, nothing will show.
If you check "Use Image" but type in "Answer Text", the text will be hidden.

**Pick ONE: Either text OR image per answer**

---

## Quick Checklist for Each Question

- [ ] Question Text is filled in
- [ ] Answers Size is 4
- [ ] All 4 answers are filled (either text or image)
- [ ] Correct Answer Index is 0-3 (not 4 or higher!)
- [ ] Feedback Explanation is filled in
- [ ] If using images, all image fields are populated

---

## Saving Questions

Good news: **Questions save automatically!** As soon as you type something in the Inspector, it saves. No "Save" button needed.

You can verify they're saved by:
1. Play the game
2. Answer a few questions
3. Stop playing
4. Check if your questions are still there ✓

---

## Testing Your Questions

1. Select your **QuizProper** GameObject
2. In Inspector, check the **Question Bank Size**
3. It should say **30** (or however many you added)
4. Expand a few random elements to verify they're filled

**Then:**
1. Press **Play**
2. Fill in player info
3. Click "Start Quiz"
4. You should see your Question 1
5. Answer it and check the feedback

If feedback shows your explanation, everything is wired correctly! ✓

---

## Summary

**To add questions:**
1. Set Question Bank Size to 30
2. For each Element (0-29):
   - Fill Question Text
   - Add 4 Answers (text or image)
   - Set Correct Answer Index (0-3)
   - Fill Feedback Explanation
3. Test in Play mode

**That's it!** Your quiz is now full of questions.

---

## Pro Tips

- **Copy-Paste questions:** You can duplicate Elements in Unity by right-clicking and selecting "Copy Component", then "Paste Component As New"
- **Keyboard shortcut:** Tab moves to next field, Shift+Tab goes back
- **Bulk edit:** If you have 30 questions, start typing them all at once. It takes maybe 30-45 minutes depending on question length
- **Organize:** Keep your questions in logical order (maybe all math first, then science, etc.) so it's easier to debug

Have fun adding questions! 🎓
