// Quiz questions about the 17 Sustainable Development Goals
const quizQuestions = [
    {
        question: "What is the first Sustainable Development Goal?",
        options: ["Zero Hunger", "No Poverty", "Quality Education", "Good Health"],
        correct: 1,
        sdg: 1
    },
    {
        question: "Which SDG focuses on ending hunger and achieving food security?",
        options: ["SDG 1", "SDG 2", "SDG 3", "SDG 4"],
        correct: 1,
        sdg: 2
    },
    {
        question: "SDG 3 aims to ensure healthy lives and promote well-being for all at what ages?",
        options: ["Children only", "Adults only", "All ages", "Senior citizens only"],
        correct: 2,
        sdg: 3
    },
    {
        question: "What does SDG 4 focus on?",
        options: ["Clean Water", "Quality Education", "Gender Equality", "Clean Energy"],
        correct: 1,
        sdg: 4
    },
    {
        question: "Which SDG aims to achieve gender equality and empower all women and girls?",
        options: ["SDG 3", "SDG 4", "SDG 5", "SDG 6"],
        correct: 2,
        sdg: 5
    },
    {
        question: "SDG 6 focuses on ensuring availability and sustainable management of what?",
        options: ["Energy", "Water and Sanitation", "Food", "Education"],
        correct: 1,
        sdg: 6
    },
    {
        question: "What type of energy does SDG 7 promote?",
        options: ["Fossil fuels", "Nuclear only", "Affordable and Clean Energy", "Coal energy"],
        correct: 2,
        sdg: 7
    },
    {
        question: "SDG 8 promotes sustained, inclusive economic growth and what?",
        options: ["Full employment", "Decent work for all", "Economic growth only", "Both A and B"],
        correct: 3,
        sdg: 8
    },
    {
        question: "Which SDG focuses on industry, innovation and infrastructure?",
        options: ["SDG 7", "SDG 8", "SDG 9", "SDG 10"],
        correct: 2,
        sdg: 9
    },
    {
        question: "SDG 10 aims to reduce what?",
        options: ["Population", "Inequalities", "Resources", "Technology"],
        correct: 1,
        sdg: 10
    },
    {
        question: "Which SDG is about making cities and human settlements inclusive and sustainable?",
        options: ["SDG 9", "SDG 10", "SDG 11", "SDG 12"],
        correct: 2,
        sdg: 11
    },
    {
        question: "SDG 12 focuses on ensuring sustainable patterns of what?",
        options: ["Transportation", "Consumption and Production", "Education", "Healthcare"],
        correct: 1,
        sdg: 12
    },
    {
        question: "Which SDG calls for urgent action to combat climate change?",
        options: ["SDG 12", "SDG 13", "SDG 14", "SDG 15"],
        correct: 1,
        sdg: 13
    },
    {
        question: "SDG 14 aims to conserve and sustainably use what?",
        options: ["Forests", "Oceans and Marine Resources", "Mountains", "Deserts"],
        correct: 1,
        sdg: 14
    },
    {
        question: "Which SDG focuses on life on land and protecting ecosystems?",
        options: ["SDG 13", "SDG 14", "SDG 15", "SDG 16"],
        correct: 2,
        sdg: 15
    },
    {
        question: "SDG 16 promotes peaceful and inclusive societies for what?",
        options: ["Economic growth", "Sustainable development", "Population control", "Resource extraction"],
        correct: 1,
        sdg: 16
    },
    {
        question: "What does SDG 17 focus on?",
        options: ["Individual action", "Partnerships for the Goals", "National policies only", "Private sector only"],
        correct: 1,
        sdg: 17
    },
    {
        question: "How many Sustainable Development Goals are there in total?",
        options: ["15", "17", "20", "25"],
        correct: 1,
        sdg: 0
    },
    {
        question: "By what year do the UN member states aim to achieve the SDGs?",
        options: ["2025", "2030", "2040", "2050"],
        correct: 1,
        sdg: 0
    },
    {
        question: "The SDGs were adopted by all UN member states in which year?",
        options: ["2010", "2012", "2015", "2020"],
        correct: 2,
        sdg: 0
    }
];

// Game state
let currentQuestionIndex = 0;
let score = 0;
let correctAnswers = 0;
let timer;
let timeLeft = 15;
const TIME_PER_QUESTION = 15;
const POINTS_PER_CORRECT = 10;
const TIME_BONUS_MULTIPLIER = 1;

// DOM elements
const startScreen = document.getElementById('start-screen');
const learnScreen = document.getElementById('learn-screen');
const quizScreen = document.getElementById('quiz-screen');
const resultScreen = document.getElementById('result-screen');

const startBtn = document.getElementById('start-btn');
const learnBtn = document.getElementById('learn-btn');
const backBtn = document.getElementById('back-btn');
const restartBtn = document.getElementById('restart-btn');
const learnMoreBtn = document.getElementById('learn-more-btn');

const questionNumber = document.getElementById('question-number');
const scoreElement = document.getElementById('score');
const timerElement = document.getElementById('timer');
const questionText = document.getElementById('question-text');
const optionsContainer = document.getElementById('options-container');
const progressBar = document.getElementById('progress');

const finalScore = document.getElementById('final-score');
const correctAnswersElement = document.getElementById('correct-answers');
const accuracyElement = document.getElementById('accuracy');
const performanceMessage = document.getElementById('performance-message');

// Event listeners
startBtn.addEventListener('click', startQuiz);
learnBtn.addEventListener('click', showLearnScreen);
backBtn.addEventListener('click', showStartScreen);
restartBtn.addEventListener('click', startQuiz);
learnMoreBtn.addEventListener('click', showLearnScreen);

// Show a specific screen
function showScreen(screen) {
    document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
    screen.classList.add('active');
}

function showStartScreen() {
    showScreen(startScreen);
}

function showLearnScreen() {
    showScreen(learnScreen);
}

function showQuizScreen() {
    showScreen(quizScreen);
}

function showResultScreen() {
    showScreen(resultScreen);
}

// Start the quiz
function startQuiz() {
    currentQuestionIndex = 0;
    score = 0;
    correctAnswers = 0;
    
    // Shuffle questions for variety
    shuffleArray(quizQuestions);
    
    showQuizScreen();
    displayQuestion();
}

// Shuffle array (Fisher-Yates algorithm)
function shuffleArray(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
}

// Display current question
function displayQuestion() {
    if (currentQuestionIndex >= quizQuestions.length) {
        endQuiz();
        return;
    }

    const question = quizQuestions[currentQuestionIndex];
    
    // Update question number and progress
    questionNumber.textContent = `${currentQuestionIndex + 1}/${quizQuestions.length}`;
    const progress = ((currentQuestionIndex + 1) / quizQuestions.length) * 100;
    progressBar.style.width = `${progress}%`;
    
    // Display question
    questionText.textContent = question.question;
    
    // Clear previous options
    optionsContainer.innerHTML = '';
    
    // Create option buttons
    question.options.forEach((option, index) => {
        const optionDiv = document.createElement('div');
        optionDiv.classList.add('option');
        optionDiv.textContent = option;
        optionDiv.addEventListener('click', () => selectOption(index));
        optionsContainer.appendChild(optionDiv);
    });
    
    // Start timer
    startTimer();
}

// Start countdown timer
function startTimer() {
    clearInterval(timer);
    timeLeft = TIME_PER_QUESTION;
    updateTimerDisplay();
    
    timer = setInterval(() => {
        timeLeft--;
        updateTimerDisplay();
        
        if (timeLeft <= 5) {
            timerElement.parentElement.classList.add('urgent');
        }
        
        if (timeLeft <= 0) {
            clearInterval(timer);
            handleTimeout();
        }
    }, 1000);
}

// Update timer display
function updateTimerDisplay() {
    timerElement.textContent = timeLeft;
}

// Handle option selection
function selectOption(selectedIndex) {
    clearInterval(timer);
    timerElement.parentElement.classList.remove('urgent');
    
    const question = quizQuestions[currentQuestionIndex];
    const options = document.querySelectorAll('.option');
    
    // Disable all options
    options.forEach(option => option.classList.add('disabled'));
    
    // Check if answer is correct
    if (selectedIndex === question.correct) {
        options[selectedIndex].classList.add('correct');
        correctAnswers++;
        
        // Calculate score with time bonus
        const basePoints = POINTS_PER_CORRECT;
        const timeBonus = Math.floor(timeLeft * TIME_BONUS_MULTIPLIER);
        const points = basePoints + timeBonus;
        score += points;
        
        scoreElement.textContent = score;
    } else {
        options[selectedIndex].classList.add('incorrect');
        options[question.correct].classList.add('correct');
    }
    
    // Move to next question after delay
    setTimeout(() => {
        currentQuestionIndex++;
        displayQuestion();
    }, 1500);
}

// Handle timeout (no answer selected)
function handleTimeout() {
    const question = quizQuestions[currentQuestionIndex];
    const options = document.querySelectorAll('.option');
    
    // Disable all options and show correct answer
    options.forEach(option => option.classList.add('disabled'));
    options[question.correct].classList.add('correct');
    
    // Move to next question after delay
    setTimeout(() => {
        currentQuestionIndex++;
        displayQuestion();
    }, 1500);
}

// End the quiz and show results
function endQuiz() {
    const totalQuestions = quizQuestions.length;
    const accuracy = Math.round((correctAnswers / totalQuestions) * 100);
    
    finalScore.textContent = score;
    correctAnswersElement.textContent = `${correctAnswers}/${totalQuestions}`;
    accuracyElement.textContent = `${accuracy}%`;
    
    // Performance message
    let message = '';
    if (accuracy >= 90) {
        message = '🌟 Outstanding! You\'re an SDG expert!';
    } else if (accuracy >= 70) {
        message = '👏 Great job! You have strong knowledge of the SDGs!';
    } else if (accuracy >= 50) {
        message = '👍 Good effort! Keep learning about the SDGs!';
    } else {
        message = '📚 Keep studying! Review the SDGs to improve your knowledge!';
    }
    
    performanceMessage.textContent = message;
    
    showResultScreen();
}

// Initialize the app
document.addEventListener('DOMContentLoaded', () => {
    showStartScreen();
});
