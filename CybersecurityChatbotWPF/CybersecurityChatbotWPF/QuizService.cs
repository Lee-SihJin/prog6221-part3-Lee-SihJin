// QuizService.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbotWPF
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public string Topic { get; set; }

        public QuizQuestion(string question, List<string> options, int correctIndex, string explanation, string topic)
        {
            Question = question;
            Options = options;
            CorrectAnswerIndex = correctIndex;
            Explanation = explanation;
            Topic = topic;
        }
    }

    public class QuizService
    {
        private List<QuizQuestion> _questions;
        private Random _random;
        private int _currentQuestionIndex;
        private int _score;
        private bool _isQuizActive;

        public bool IsQuizActive => _isQuizActive;
        public int CurrentQuestionIndex => _currentQuestionIndex;
        public int TotalQuestions => _questions.Count;
        public int Score => _score;

        public QuizService()
        {
            _random = new Random();
            InitializeQuestions();
            ResetQuiz();
        }

        private void InitializeQuestions()
        {
            _questions = new List<QuizQuestion>
            {
                // Password Safety Questions
                new QuizQuestion(
                    "What is the recommended minimum length for a strong password?",
                    new List<string> { "6 characters", "8 characters", "12 characters", "16 characters" },
                    2,
                    "Security experts recommend at least 12 characters for strong passwords. Longer passwords are harder to crack!",
                    "Password"
                ),
                new QuizQuestion(
                    "Which of the following is the BEST practice for managing passwords?",
                    new List<string> { "Use the same password everywhere", "Write passwords on sticky notes", "Use a password manager", "Use your pet's name" },
                    2,
                    "Password managers generate and store unique, strong passwords for each account, making them the safest option.",
                    "Password"
                ),
                new QuizQuestion(
                    "What does 2FA stand for?",
                    new List<string> { "Two Factor Authentication", "Two Form Access", "Triple Factor Authentication", "Two File Access" },
                    0,
                    "2FA stands for Two Factor Authentication - it adds an extra layer of security by requiring a second form of verification.",
                    "Password"
                ),
                new QuizQuestion(
                    "What is a passphrase?",
                    new List<string> { "A short password", "A sequence of random words", "A password with numbers only", "A single word" },
                    1,
                    "Passphrases like 'correct-horse-battery-staple' are long sequences of random words that are easy to remember but hard to crack.",
                    "Password"
                ),

                // Phishing Questions
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new List<string> { "Reply with your password", "Delete the email", "Report it as phishing", "Forward it to friends" },
                    2,
                    "Reporting phishing emails helps prevent scams. Legitimate companies never ask for passwords via email.",
                    "Phishing"
                ),
                new QuizQuestion(
                    "Which of these is a sign of a phishing email?",
                    new List<string> { "Personalized greeting", "Urgent language demanding action", "Professional formatting", "Correct grammar" },
                    1,
                    "Urgent language demanding immediate action is a common phishing tactic used to pressure victims.",
                    "Phishing"
                ),
                new QuizQuestion(
                    "What is 'smishing'?",
                    new List<string> { "Phishing via SMS", "Phishing via email", "Phishing via phone calls", "Phishing via social media" },
                    0,
                    "Smishing is phishing conducted through SMS/text messages. Scammers use texts to trick victims.",
                    "Phishing"
                ),

                // Safe Browsing Questions
                new QuizQuestion(
                    "What does the padlock icon in your browser's address bar indicate?",
                    new List<string> { "The site is safe", "The connection is encrypted", "The site is popular", "The site is fast" },
                    1,
                    "The padlock icon indicates an encrypted HTTPS connection, meaning your data is protected in transit.",
                    "Browsing"
                ),
                new QuizQuestion(
                    "Why should you avoid using public Wi-Fi for banking?",
                    new List<string> { "It's too slow", "It uses too much data", "It may not be secure", "It costs money" },
                    2,
                    "Public Wi-Fi networks are often unsecured, making it easier for hackers to intercept your data.",
                    "Browsing"
                ),
                new QuizQuestion(
                    "What is the safest way to connect to the internet in public places?",
                    new List<string> { "Use free public Wi-Fi", "Use a VPN", "Use mobile hotspot", "Both B and C are good options" },
                    3,
                    "Both using a VPN and using your mobile hotspot are safer than public Wi-Fi. VPNs encrypt your connection.",
                    "Browsing"
                ),

                // Privacy Questions
                new QuizQuestion(
                    "What should you review regularly to protect your online privacy?",
                    new List<string> { "Privacy settings on social media", "App permissions on your phone", "Browser cookie settings", "All of the above" },
                    3,
                    "Regularly reviewing all privacy-related settings helps maintain control over your personal information.",
                    "Privacy"
                ),
                new QuizQuestion(
                    "Why is it important to use different emails for different purposes?",
                    new List<string> { "It looks more professional", "It reduces spam and tracking", "It saves storage", "It's required by law" },
                    1,
                    "Using separate emails for personal, shopping, and newsletters reduces spam and limits tracking.",
                    "Privacy"
                )
            };
        }

        public void ResetQuiz()
        {
            _currentQuestionIndex = 0;
            _score = 0;
            _isQuizActive = false;
        }

        public QuizQuestion StartQuiz()
        {
            _isQuizActive = true;
            _currentQuestionIndex = 0;
            _score = 0;

            // Shuffle questions
            _questions = _questions.OrderBy(x => _random.Next()).ToList();
            return _questions.FirstOrDefault();
        }

        public QuizQuestion GetNextQuestion()
        {
            _currentQuestionIndex++;
            if (_currentQuestionIndex < _questions.Count)
            {
                return _questions[_currentQuestionIndex];
            }
            return null;
        }

        public bool CheckAnswer(int answerIndex, out bool isCorrect, out string explanation)
        {
            var question = _questions[_currentQuestionIndex];
            isCorrect = answerIndex == question.CorrectAnswerIndex;
            explanation = question.Explanation;

            if (isCorrect)
            {
                _score++;
            }

            return isCorrect;
        }

        public QuizQuestion GetCurrentQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                return _questions[_currentQuestionIndex];
            }
            return null;
        }

        public string GetScoreFeedback()
        {
            double percentage = (double)_score / _questions.Count * 100;

            if (percentage >= 90)
                return "Outstanding! You're a Cybersecurity Pro! Your knowledge is impressive!";
            else if (percentage >= 70)
                return "Great job! You have strong cybersecurity awareness! Keep learning!";
            else if (percentage >= 50)
                return "Good effort! There's room for improvement. Review the topics you missed!";
            else
                return "Keep learning! Cybersecurity is important. Review the basics and try again!";
        }

        public int GetQuestionCount()
        {
            return _questions.Count;
        }
    }
}

// quiz answer:220121012331