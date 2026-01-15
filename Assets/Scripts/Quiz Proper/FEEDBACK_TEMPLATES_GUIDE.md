# FEEDBACK TEMPLATES - CUSTOMIZATION GUIDE

## Overview

The quiz now has **3 customizable feedback templates** for different scenarios:
1. **Correct Answer** - Player got the question right
2. **Wrong Answer** - Player got the question wrong
3. **Time's Up** - Player ran out of time

Each template has **2 sentences** with placeholders you can customize!

---

## Default Templates

### Correct Answer Feedback
```
Correct! The answer is [ANSWER]. It is because [REASON]
```

### Wrong Answer Feedback
```
Incorrect. The correct answer is [ANSWER]. It is because [REASON]
```

### Time's Up Feedback
```
TIME'S UP! The correct answer is [ANSWER]. It is because [REASON]
```

---

## Placeholder Guide

Each template uses **2 placeholders** that are automatically filled in:

| Placeholder | What It Is | Example |
|-------------|-----------|---------|
| `[ANSWER]` | The text of the correct answer | "Photosynthesis" |
| `[REASON]` | The feedback explanation from the question | "Plants use sunlight to convert CO2 into glucose" |

---

## How to Customize (Inspector)

### Step 1: Select QuizProper GameObject
1. In Hierarchy, click **QuizProper**
2. In Inspector, find **Feedback Templates** section

### Step 2: Edit the Templates

You'll see 3 text fields:
- **Correct Answer Template**
- **Wrong Answer Template**
- **Time Up Template**

Each field shows a text area where you can type your own template!

### Example Customizations

**Option 1: Encouraging Tone**
```
Correct Answer Template: "Amazing! The answer is [ANSWER]. Here's why: [REASON]"
Wrong Answer Template: "Not quite! The correct answer is [ANSWER]. Remember: [REASON]"
Time Up Template: "Time flew! The answer was [ANSWER]. Good to know: [REASON]"
```

**Option 2: Educational Tone**
```
Correct Answer Template: "Correct! The answer is [ANSWER]. [REASON]"
Wrong Answer Template: "Incorrect. The answer is [ANSWER]. [REASON]"
Time Up Template: "Time expired. The answer is [ANSWER]. [REASON]"
```

**Option 3: Simplified Tone**
```
Correct Answer Template: "✓ Right! [ANSWER]. [REASON]"
Wrong Answer Template: "✗ Wrong! [ANSWER]. [REASON]"
Time Up Template: "⏱ Out of time! [ANSWER]. [REASON]"
```

---

## Rules for Creating Templates

### ✅ DO:
- Use `[ANSWER]` and `[REASON]` in your templates
- Use newlines for spacing: `\n` creates a line break
- Keep it 2 sentences max for readability
- Use encouraging language

### ❌ DON'T:
- Remove the placeholders (they auto-fill)
- Make templates too long (screen space is limited)
- Use special characters that might not display
- Forget the spacing between sentences

---

## Live Example

Let's say you have this question:

**Question:** "What is the capital of France?"
- Correct Answer: "Paris"
- Feedback Explanation: "Paris is the largest city and capital of France"

**With Default Template:**
```
Correct! The answer is Paris. It is because Paris is the largest city and capital of France
```

**With Custom Template (Encouraging):**
```
Great job! The answer is Paris. Here's why: Paris is the largest city and capital of France
```

Both show the same information, but the tone is different!

---

## Step-by-Step Customization

### Example: Create a Fun Template

1. **Select QuizProper** in Hierarchy
2. Find **Feedback Templates** in Inspector
3. Click on **Correct Answer Template** text field
4. Clear it and type:
   ```
   🎉 Awesome! The answer is [ANSWER]. [REASON]
   ```
5. Do the same for **Wrong Answer Template**:
   ```
   😔 Oops! The right answer is [ANSWER]. [REASON]
   ```
6. And **Time Up Template**:
   ```
   ⏰ Out of time! The answer is [ANSWER]. [REASON]
   ```
7. **Save your scene**
8. **Play** to test!

---

## Testing Your Templates

1. **Play the game** in Unity
2. **Answer a question correctly** → See correct feedback
3. **Answer a question wrong** → See wrong feedback
4. **Let time run out** → See time's up feedback
5. **Check the text** → Make sure it shows `[ANSWER]` replaced with actual answer
6. **Check clarity** → Is the feedback readable on screen?

---

## Common Mistakes & Fixes

### Problem: `[ANSWER]` or `[REASON]` showing in feedback

**Cause:** The placeholders are spelled wrong (case-sensitive!)

**Fix:** Make sure it's exactly:
- `[ANSWER]` (all caps, with square brackets)
- `[REASON]` (all caps, with square brackets)

✅ Correct: `"The answer is [ANSWER]"`  
❌ Wrong: `"The answer is [answer]"` or `"The answer is {ANSWER}"`

---

### Problem: Feedback text is cut off

**Cause:** Template is too long for the screen

**Fix:** Shorten the template:
```
Before: "Congratulations! You got this one correct! The answer is [ANSWER]. It is because [REASON]"
After: "Correct! The answer is [ANSWER]. [REASON]"
```

---

### Problem: Feedback looks weird on mobile

**Cause:** Special characters don't render well on all phones

**Fix:** Use only basic English text:
```
Before: "✨ Amazing! The answer is [ANSWER]! 🎉"
After: "Great! The answer is [ANSWER]."
```

---

## Technical Details (For Developers)

### How Feedback Works

In `QuizProper.cs`, the system:

1. **Gets the template** based on feedback type:
   ```csharp
   if (feedbackType == "CORRECT")
       template = correctAnswerTemplate;
   ```

2. **Replaces placeholders** with actual values:
   ```csharp
   string fullFeedback = template
       .Replace("[ANSWER]", correctAnswerText)
       .Replace("[REASON]", question.feedbackExplanation);
   ```

3. **Shows the result** in the question label:
   ```csharp
   questionLabel.text = fullFeedback;
   ```

### Inspector Fields

The 3 templates are stored as:
- `correctAnswerTemplate` - Used when player answers correctly
- `wrongAnswerTemplate` - Used when player answers incorrectly
- `timeUpTemplate` - Used when time runs out

Each is marked as `[TextArea(2, 3)]` in code, allowing 2-3 lines in Inspector.

---

## Best Practices

### For Teachers/Content Creators

✅ **Keep it simple:** Students should understand the feedback quickly  
✅ **Be consistent:** Use similar structure in all 3 templates  
✅ **Be encouraging:** Even wrong answers should feel like learning opportunities  
✅ **Be clear:** Feedback should teach, not confuse  

### Example: Good Feedback Set

```
Correct: "Correct! The answer is [ANSWER]. [REASON]"
Wrong: "Incorrect. The answer is [ANSWER]. [REASON]"
Time Up: "Time's up! The answer is [ANSWER]. [REASON]"
```

Simple, consistent, clear.

---

## Troubleshooting Checklist

- [ ] Feedback templates are filled in (not blank)
- [ ] Placeholders are exactly `[ANSWER]` and `[REASON]` (case matters!)
- [ ] Templates fit on screen (not too long)
- [ ] All 3 templates are customized (Correct, Wrong, Time Up)
- [ ] Scene is saved after making changes
- [ ] Game tested in Play mode

---

## Next Steps

1. **Customize the 3 templates** in QuizProper Inspector
2. **Test them** by playing a quiz
3. **Adjust** if text is too long or unclear
4. **Save and commit** your changes

---

**Tip:** Different feedback can improve learning! Encouraging feedback for correct answers, educational feedback for wrong answers, and informative feedback for time's up creates a better learning experience.

Good luck customizing your feedback! 🎓

