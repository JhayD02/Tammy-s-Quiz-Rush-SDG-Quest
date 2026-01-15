// This script holds the data structures for questions and answers
// It doesn't do anything - it just stores information
// Think of it like a template for how questions should look

using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AnswerChoice
{
    [SerializeField] public string answerText;
    [SerializeField] public Sprite answerImage;
    [SerializeField] public bool useImage = false; // true = show image, false = show text
}

[System.Serializable]
public class QuizQuestion
{
    [SerializeField] public string questionText;
    [SerializeField] public List<AnswerChoice> answers = new List<AnswerChoice>(4);
    [SerializeField] public int correctAnswerIndex; // 0, 1, 2, or 3 - which answer is correct
    
    [Header("=== FEEDBACK MESSAGES ===")]
    [Tooltip("Message shown when player answers correctly")]
    [TextArea(2, 3)]
    [SerializeField] public string correctFeedback = "Correct!";
    
    [Tooltip("Message shown when player answers incorrectly")]
    [TextArea(2, 3)]
    [SerializeField] public string wrongFeedback = "Incorrect.";
    
    [Tooltip("Message shown when time runs out")]
    [TextArea(2, 3)]
    [SerializeField] public string timeUpFeedback = "Time's up!";
}
