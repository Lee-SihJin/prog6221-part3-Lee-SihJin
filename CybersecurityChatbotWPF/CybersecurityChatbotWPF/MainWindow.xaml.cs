using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbotWPF
{
    public partial class MainWindow : Window
    {
        private ChatbotService _chatbotService;
        private AudioService _audioService;
        private DatabaseHelper _dbHelper;
        private QuizService _quizService;
        private ActivityLog _activityLog;
        private Dictionary<string, List<string>> _conversationMemory;
        private string _currentTopic;
        private bool _waitingForName = true;
        private bool _isQuizActive = false;
        private bool _awaitingTaskConfirmation = false;
        private TaskItem _pendingTask;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            PlayVoiceGreeting();
            ShowWelcomeMessage();

            _activityLog.AddEntry("Application", "Cybersecurity Awareness Bot started");
        }

        private void InitializeServices()
        {
            _chatbotService = new ChatbotService();
            _audioService = new AudioService();
            _dbHelper = new DatabaseHelper();
            _quizService = new QuizService();
            _activityLog = new ActivityLog();
            _conversationMemory = new Dictionary<string, List<string>>();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                _audioService.PlayGreeting();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
            }
        }

        private async void ShowWelcomeMessage()
        {
            await Task.Delay(500);
            AddBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot! I'm here to help you stay safe online.", "Greeting");
            await Task.Delay(500);
            AddBotMessage("What's your name?", "Greeting");
            _waitingForName = true;
        }

        private void AddUserMessage(string message)
        {
            var messageBorder = new Border
            {
                Style = (Style)FindResource("ChatBubbleUser"),
                Margin = new Thickness(10, 5, 50, 5)
            };

            var stackPanel = new StackPanel();

            var nameText = new TextBlock
            {
                Text = _chatbotService.GetUserName() ?? "You",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 150, 200)),
                Margin = new Thickness(0, 0, 0, 3)
            };

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400
            };

            var timeText = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                Margin = new Thickness(0, 3, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            stackPanel.Children.Add(nameText);
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(timeText);
            messageBorder.Child = stackPanel;

            ChatStackPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        private void AddBotMessage(string message, string topic)
        {
            var messageBorder = new Border
            {
                Style = (Style)FindResource("ChatBubbleBot"),
                Margin = new Thickness(50, 5, 10, 5)
            };

            var stackPanel = new StackPanel();

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var botIcon = new TextBlock
            {
                Text = "BOT",
                FontSize = 12,
                Margin = new Thickness(0, 0, 5, 0),
                Foreground = new SolidColorBrush(Color.FromRgb(100, 200, 100))
            };
            var topicText = new TextBlock
            {
                Text = $"[{topic}]",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 100)),
                Margin = new Thickness(0, 0, 0, 3)
            };
            headerPanel.Children.Add(botIcon);
            headerPanel.Children.Add(topicText);

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400
            };

            var timeText = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                Margin = new Thickness(0, 3, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(headerPanel);
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(timeText);
            messageBorder.Child = stackPanel;

            ChatStackPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        private void AddSystemMessage(string message)
        {
            var messageBorder = new Border
            {
                Style = (Style)FindResource("SystemMessageStyle")
            };

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                TextAlignment = TextAlignment.Center,
                FontStyle = FontStyles.Italic
            };

            messageBorder.Child = messageText;
            ChatStackPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        private void AddQuizQuestion(QuizQuestion question, int questionNumber)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 60)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(30, 8, 30, 8),
                Padding = new Thickness(15, 10, 15, 10)
            };

            var stackPanel = new StackPanel();

            var header = new TextBlock
            {
                Text = $"Question {questionNumber}/{_quizService.TotalQuestions}: {question.Topic}",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 100)),
                Margin = new Thickness(0, 0, 0, 8)
            };
            stackPanel.Children.Add(header);

            var questionText = new TextBlock
            {
                Text = question.Question,
                FontSize = 14,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stackPanel.Children.Add(questionText);

            var optionsPanel = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
            for (int i = 0; i < question.Options.Count; i++)
            {
                int optionIndex = i;
                var optionButton = new Button
                {
                    Content = $"{Convert.ToChar(65 + i)}. {question.Options[i]}",
                    Background = new SolidColorBrush(Color.FromRgb(50, 50, 70)),
                    Foreground = Brushes.White,
                    FontSize = 13,
                    Padding = new Thickness(10, 6, 10, 6),
                    Margin = new Thickness(0, 3, 0, 3),
                    Cursor = Cursors.Hand,
                    Tag = optionIndex,
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 90)),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinWidth = 300
                };
                optionButton.Click += (s, e) => HandleQuizAnswer(optionIndex);
                optionsPanel.Children.Add(optionButton);
            }
            stackPanel.Children.Add(optionsPanel);

            border.Child = stackPanel;
            ChatStackPanel.Children.Add(border);
            ScrollToBottom();
        }

        private async void HandleQuizAnswer(int selectedIndex)
        {
            if (!_isQuizActive) return;

            var currentQuestion = _quizService.GetCurrentQuestion();
            if (currentQuestion == null) return;

            bool isCorrect = _quizService.CheckAnswer(selectedIndex, out bool correct, out string explanation);

            if (isCorrect)
            {
                AddSystemMessage($"Correct! {explanation}");
                _activityLog.AddEntry("Quiz", $"Correct answer: {currentQuestion.Question}");
            }
            else
            {
                string correctAnswer = currentQuestion.Options[currentQuestion.CorrectAnswerIndex];
                AddSystemMessage($"Incorrect. The correct answer was: {correctAnswer}");
                AddSystemMessage($"{explanation}");
                _activityLog.AddEntry("Quiz", $"Incorrect answer: {currentQuestion.Question}");
            }

            await Task.Delay(1000);

            var nextQuestion = _quizService.GetNextQuestion();
            if (nextQuestion != null)
            {
                AddQuizQuestion(nextQuestion, _quizService.CurrentQuestionIndex + 1);
            }
            else
            {
                _isQuizActive = false;
                string feedback = _quizService.GetScoreFeedback();
                AddSystemMessage($"Quiz Complete! Your score: {_quizService.Score}/{_quizService.TotalQuestions}");
                AddBotMessage(feedback, "Quiz Complete");
                _activityLog.AddEntry("Quiz", $"Completed with score {_quizService.Score}/{_quizService.TotalQuestions}");
                _quizService.ResetQuiz();
            }
        }

        private void AddTaskDisplay(List<TaskItem> tasks)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 40, 30)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(30, 8, 30, 8),
                Padding = new Thickness(15, 10, 15, 10)
            };

            var stackPanel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Your Tasks",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 200, 100)),
                Margin = new Thickness(0, 0, 0, 10)
            };
            stackPanel.Children.Add(header);

            if (tasks.Count == 0)
            {
                var emptyText = new TextBlock
                {
                    Text = "No tasks yet. Add a task to get started!",
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                    FontStyle = FontStyles.Italic
                };
                stackPanel.Children.Add(emptyText);
            }
            else
            {
                foreach (var task in tasks)
                {
                    var taskPanel = new Border
                    {
                        Margin = new Thickness(0, 5, 0, 5),
                        Background = task.IsCompleted ?
                            new SolidColorBrush(Color.FromRgb(40, 50, 40)) :
                            new SolidColorBrush(Color.FromRgb(45, 45, 45)),
                        Padding = new Thickness(10, 5, 10, 5),
                        CornerRadius = new CornerRadius(5)
                    };

                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var taskText = new TextBlock
                    {
                        Text = task.DisplayText,
                        FontSize = 13,
                        Foreground = task.IsCompleted ?
                            new SolidColorBrush(Color.FromRgb(150, 200, 150)) :
                            Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 5, 10, 5)
                    };
                    Grid.SetColumn(taskText, 0);
                    grid.Children.Add(taskText);

                    var buttonPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    if (!task.IsCompleted)
                    {
                        var completeButton = new Button
                        {
                            Content = "Done",
                            Background = new SolidColorBrush(Color.FromRgb(0, 100, 0)),
                            Foreground = Brushes.White,
                            FontSize = 11,
                            Padding = new Thickness(8, 4, 8, 4),
                            Margin = new Thickness(0, 0, 5, 0),
                            Cursor = Cursors.Hand,
                            Tag = task.Id,
                            BorderThickness = new Thickness(0)
                        };
                        completeButton.Click += (s, e) => MarkTaskComplete((int)((Button)s).Tag);
                        buttonPanel.Children.Add(completeButton);
                    }

                    var deleteButton = new Button
                    {
                        Content = "Delete",
                        Background = new SolidColorBrush(Color.FromRgb(100, 0, 0)),
                        Foreground = Brushes.White,
                        FontSize = 11,
                        Padding = new Thickness(8, 4, 8, 4),
                        Cursor = Cursors.Hand,
                        Tag = task.Id,
                        BorderThickness = new Thickness(0)
                    };
                    deleteButton.Click += (s, e) => DeleteTask((int)((Button)s).Tag);
                    buttonPanel.Children.Add(deleteButton);

                    Grid.SetColumn(buttonPanel, 1);
                    grid.Children.Add(buttonPanel);

                    taskPanel.Child = grid;
                    stackPanel.Children.Add(taskPanel);
                }
            }

            border.Child = stackPanel;
            ChatStackPanel.Children.Add(border);
            ScrollToBottom();
        }

        private async void MarkTaskComplete(int taskId)
        {
            if (_dbHelper.UpdateTaskStatus(taskId, true))
            {
                _activityLog.AddEntry("Task", $"Task {taskId} marked as complete");
                AddSystemMessage("Task marked as complete!");
                await Task.Delay(300);
                ViewTasks_Click(null, null);
            }
        }

        private async void DeleteTask(int taskId)
        {
            if (_dbHelper.DeleteTask(taskId))
            {
                _activityLog.AddEntry("Task", $"Task {taskId} deleted");
                AddSystemMessage("Task deleted successfully!");
                await Task.Delay(300);
                ViewTasks_Click(null, null);
            }
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToBottom();
        }

        // FEATURE BUTTON HANDLERS

        private void ViewTasks_Click(object sender, RoutedEventArgs e)
        {
            var tasks = _dbHelper.GetTasks(false);
            AddTaskDisplay(tasks);
            _activityLog.AddEntry("View", "User viewed tasks");
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (_isQuizActive) return;

            _isQuizActive = true;
            _quizService.ResetQuiz();
            var firstQuestion = _quizService.StartQuiz();
            AddSystemMessage("Starting Cybersecurity Quiz! Answer each question to test your knowledge.");
            _activityLog.AddEntry("Quiz", "Quiz started");
            AddQuizQuestion(firstQuestion, 1);
        }

        private void ShowActivityLog_Click(object sender, RoutedEventArgs e)
        {
            string logSummary = _activityLog.GetSummary(10);
            AddBotMessage(logSummary, "Activity Log");
            _activityLog.AddEntry("View", "User viewed activity log");
        }

        private void ShowHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage =
                "**Cybersecurity Awareness Bot - Help**\n\n" +
                "**Topics I can help with:**\n" +
                "• Password safety & 2FA\n" +
                "• Phishing & scam protection\n" +
                "• Safe browsing & public Wi-Fi\n" +
                "• Privacy & data protection\n\n" +
                "**Commands you can use:**\n" +
                "• 'add task [description]' - Add a cybersecurity task\n" +
                "• 'remind me [task] in [X days]' - Set a reminder\n" +
                "• 'view tasks' - Show all tasks\n" +
                "• 'quiz' - Start the cybersecurity quiz\n" +
                "• 'activity log' - View recent actions\n\n" +
                "**Try asking:**\n" +
                "• 'How do I create strong passwords?'\n" +
                "• 'What is phishing?'\n" +
                "• 'Tips for safe browsing'\n" +
                "• 'How to protect my privacy?'";

            AddBotMessage(helpMessage, "Help");
            _activityLog.AddEntry("Help", "User viewed help menu");
        }

        // ===== SEND BUTTON AND INPUT HANDLING =====

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await ProcessUserInput();
            }
        }

        // ===== MAIN PROCESSING METHOD =====

        private async Task ProcessUserInput()
        {
            string userInput = MessageTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            MessageTextBox.Text = "";
            AddUserMessage(userInput);

            System.Diagnostics.Debug.WriteLine($"===== PROCESSING: '{userInput}' =====");
            System.Diagnostics.Debug.WriteLine($"AWAITING TASK CONFIRMATION: {_awaitingTaskConfirmation}");

            // ===== HANDLE TASK CONFIRMATION (yes/no for reminder) =====
            if (_awaitingTaskConfirmation && _pendingTask != null)
            {
                string lowerInput = userInput.ToLower();

                if (lowerInput == "yes" || lowerInput == "y" || lowerInput == "sure" ||
                    lowerInput == "ok" || lowerInput == "okay" || lowerInput == "yeah")
                {
                    // User wants a reminder
                    DateTime reminderDate = DateTime.Now.AddDays(3);
                    _dbHelper.AddTask(_pendingTask.Title, _pendingTask.Description, reminderDate);
                    _dbHelper.DeleteTask(_pendingTask.Id); // Delete the old one without reminder

                    _activityLog.AddEntry("Task", $"Added task with reminder: {_pendingTask.Title}");
                    AddBotMessage($"✅ Task added: '{_pendingTask.Title}' with reminder set for {reminderDate:yyyy-MM-dd}.", "Task");

                    _awaitingTaskConfirmation = false;
                    _pendingTask = null;
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    return;
                }
                else if (lowerInput == "no" || lowerInput == "n" || lowerInput == "nah" ||
                         lowerInput == "never" || lowerInput == "not really")
                {
                    // User doesn't want a reminder
                    _activityLog.AddEntry("Task", $"Added task without reminder: {_pendingTask.Title}");
                    AddBotMessage($"✅ Task added: '{_pendingTask.Title}' (no reminder set).", "Task");

                    _awaitingTaskConfirmation = false;
                    _pendingTask = null;
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    return;
                }
                else
                {
                    // User said something else - ask again
                    AddBotMessage("I didn't understand. Would you like to set a reminder for this task? (say 'yes' or 'no')", "Task");
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    return;
                }
            }

            // Handle name input if waiting for name
            if (_waitingForName)
            {
                _chatbotService.SetUserName(userInput);
                _waitingForName = false;

                await Task.Delay(500);
                AddBotMessage($"Nice to meet you, {userInput}! I'm your Cybersecurity Awareness Assistant.", "Greeting");
                await Task.Delay(500);
                AddBotMessage("I can help with passwords, phishing, safe browsing, and privacy. I also have a Task Assistant and Quiz! Type 'help' to see what I can do.", "Help");
                _activityLog.AddEntry("User", $"User registered: {userInput}");
                return;
            }

            // Check for quiz answers when quiz is active
            if (_isQuizActive)
            {
                return;
            }

            LoadingOverlay.Visibility = Visibility.Visible;
            await Task.Delay(300);

            // Check for commands using NLP
            var parsedAction = _chatbotService.ParseUserIntent(userInput);

            System.Diagnostics.Debug.WriteLine($"PARSED ACTION: {parsedAction.Action}, Subject: '{parsedAction.Subject}'");

            if (parsedAction.Action == "AddTask" || parsedAction.Action == "AddReminder")
            {
                await HandleTaskCommand(parsedAction, userInput);
            }
            else if (parsedAction.Action == "ViewTasks")
            {
                ViewTasks_Click(null, null);
            }
            else if (parsedAction.Action == "StartQuiz")
            {
                StartQuiz_Click(null, null);
            }
            else if (parsedAction.Action == "ShowLog")
            {
                ShowActivityLog_Click(null, null);
            }
            else if (parsedAction.Action == "CompleteTask")
            {
                var tasks = _dbHelper.GetTasks(false);
                var matchingTask = tasks.FirstOrDefault(t =>
                    t.Title.ToLower().Contains(parsedAction.Subject.ToLower()) ||
                    parsedAction.Subject.ToLower().Contains(t.Title.ToLower()));

                if (matchingTask != null)
                {
                    MarkTaskComplete(matchingTask.Id);
                }
                else
                {
                    AddBotMessage($"I couldn't find a task matching '{parsedAction.Subject}'. Use 'view tasks' to see your tasks.", "Task");
                }
            }
            else if (parsedAction.Action == "DeleteTask")
            {
                var tasks = _dbHelper.GetTasks(true);
                var matchingTask = tasks.FirstOrDefault(t =>
                    t.Title.ToLower().Contains(parsedAction.Subject.ToLower()) ||
                    parsedAction.Subject.ToLower().Contains(t.Title.ToLower()));

                if (matchingTask != null)
                {
                    DeleteTask(matchingTask.Id);
                }
                else
                {
                    AddBotMessage($"I couldn't find a task matching '{parsedAction.Subject}'. Use 'view tasks' to see your tasks.", "Task");
                }
            }
            else
            {
                // Regular chatbot response
                var response = await Task.Run(() => _chatbotService.GetResponse(userInput));

                string sentiment = DetectSentiment(userInput);
                string finalResponse = AdjustResponseForSentiment(response.Message, sentiment);

                if (IsFollowUpQuestion(userInput))
                {
                    finalResponse = HandleFollowUp(userInput, finalResponse);
                }

                StoreInMemory(userInput, response.Topic);

                if (userInput.ToLower().Contains("remember") || userInput.ToLower().Contains("recall") ||
                    userInput.ToLower().Contains("what did i"))
                {
                    string recalledInfo = RecallInformation(userInput);
                    if (!string.IsNullOrEmpty(recalledInfo))
                    {
                        finalResponse = recalledInfo;
                    }
                }

                if (userInput.ToLower().Contains("interested in") || userInput.ToLower().Contains("i like"))
                {
                    StoreUserInterest(userInput);
                }

                AddBotMessage(finalResponse, response.Topic);
                StoreConversationHistory(userInput, finalResponse);

                if (response.Topic == "Exit")
                {
                    await Task.Delay(1000);
                    AddBotMessage("Thank you for using the Cybersecurity Awareness Bot! Stay safe online!", "Farewell");
                    _activityLog.AddEntry("Application", "User session ended");
                }
            }

            LoadingOverlay.Visibility = Visibility.Collapsed;
        }

        // ===== TASK COMMAND HANDLER =====

        private async Task HandleTaskCommand(TaskAction action, string userInput)
        {
            System.Diagnostics.Debug.WriteLine($"TASK COMMAND: Action={action.Action}, Subject='{action.Subject}'");

            if (action.Action == "AddTask")
            {
                string title = action.Subject ?? "New Task";
                string description = userInput;

                try
                {
                    // Store the task with a flag that reminder is pending
                    // We'll add it to database only when user confirms
                    _pendingTask = new TaskItem
                    {
                        Id = 0,
                        Title = title,
                        Description = description
                    };
                    _awaitingTaskConfirmation = true;

                    AddBotMessage($"✅ Task ready: '{title}'. Would you like to set a reminder? (say 'yes' or 'no')", "Task");

                    System.Diagnostics.Debug.WriteLine($"Task pending confirmation: {title}");
                }
                catch (Exception ex)
                {
                    AddBotMessage($"❌ Error: {ex.Message}", "Error");
                    System.Diagnostics.Debug.WriteLine($"❌ Exception: {ex.Message}");
                }
            }
            else if (action.Action == "AddReminder")
            {
                string title = action.Subject ?? "Reminder";
                DateTime? reminderDate = action.ReminderDate ?? DateTime.Now.AddDays(3);
                int taskId = _dbHelper.AddTask(title, $"Reminder: {userInput}", reminderDate);
                if (taskId > 0)
                {
                    _activityLog.AddEntry("Reminder", $"Set reminder: {title} for {reminderDate:yyyy-MM-dd}");
                    AddBotMessage($"✅ Reminder set for '{title}' on {reminderDate:yyyy-MM-dd}.", "Reminder");
                    System.Diagnostics.Debug.WriteLine($"✅ Reminder added successfully! ID: {taskId}");
                }
                else
                {
                    AddBotMessage($"❌ Failed to set reminder. Please try again.", "Error");
                    System.Diagnostics.Debug.WriteLine($"❌ Failed to add reminder!");
                }
            }
        }

        // ===== SENTIMENT DETECTION METHODS =====

        private string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("worried") || lowerInput.Contains("scared") ||
                lowerInput.Contains("anxious") || lowerInput.Contains("nervous") ||
                lowerInput.Contains("fear") || lowerInput.Contains("afraid"))
            {
                return "worried";
            }
            else if (lowerInput.Contains("confused") || lowerInput.Contains("lost") ||
                     lowerInput.Contains("dont understand") || lowerInput.Contains("don't understand"))
            {
                return "confused";
            }
            else if (lowerInput.Contains("frustrated") || lowerInput.Contains("annoyed") ||
                     lowerInput.Contains("angry") || lowerInput.Contains("fed up"))
            {
                return "frustrated";
            }
            else if (lowerInput.Contains("curious") || lowerInput.Contains("interested") ||
                     lowerInput.Contains("tell me") || lowerInput.Contains("want to know"))
            {
                return "curious";
            }
            else if (lowerInput.Contains("happy") || lowerInput.Contains("great") ||
                     lowerInput.Contains("good") || lowerInput.Contains("excited"))
            {
                return "happy";
            }

            return "neutral";
        }

        private string AdjustResponseForSentiment(string response, string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "It's completely understandable to feel that way. " + response +
                           "\n\nRemember, staying informed is the first step to staying safe! You're doing the right thing by learning about this.";
                case "confused":
                    return "I understand this can be confusing. Let me explain simply: " + response +
                           "\n\nWould you like me to explain more about this topic in a different way?";
                case "frustrated":
                    return "I hear your frustration. Cybersecurity can be challenging, but you're doing great! " + response +
                           "\n\nTake a deep breath - you're building important skills that will protect you online!";
                case "curious":
                    return "Great question! I love your curiosity about cybersecurity. " + response +
                           "\n\nKeep asking questions - that's how we all learn to stay safe!";
                case "happy":
                    return "That's wonderful to hear! " + response;
                default:
                    return response;
            }
        }

        // ===== FOLLOW-UP HANDLING =====

        private bool IsFollowUpQuestion(string input)
        {
            string lowerInput = input.ToLower();
            return lowerInput.Contains("another") ||
                   lowerInput.Contains("more") ||
                   lowerInput.Contains("explain") ||
                   lowerInput.Contains("tell me more") ||
                   lowerInput.Contains("elaborate") ||
                   lowerInput.Contains("continue");
        }

        private string HandleFollowUp(string input, string currentResponse)
        {
            string lowerInput = input.ToLower();

            if ((lowerInput.Contains("another") || lowerInput.Contains("more")) &&
                !string.IsNullOrEmpty(_currentTopic))
            {
                var newResponse = _chatbotService.GetRandomResponseForTopic(_currentTopic);
                if (!string.IsNullOrEmpty(newResponse))
                {
                    return $"Sure! Here's another tip about {_currentTopic}: {newResponse}";
                }
            }
            else if (lowerInput.Contains("explain") || lowerInput.Contains("tell me more"))
            {
                return $"Of course! Let me provide more detail. {currentResponse}\n\nWould you like to know more about this topic?";
            }

            return currentResponse;
        }

        // ===== MEMORY AND RECALL METHODS =====

        private void StoreInMemory(string userInput, string topic)
        {
            if (!string.IsNullOrEmpty(topic) && topic != "General" && topic != "Invalid Input" && topic != "Exit")
            {
                _currentTopic = topic;

                if (!_conversationMemory.ContainsKey(topic))
                {
                    _conversationMemory[topic] = new List<string>();
                }

                _conversationMemory[topic].Add(userInput);

                if (_conversationMemory[topic].Count > 10)
                {
                    _conversationMemory[topic].RemoveAt(0);
                }
            }
        }

        private void StoreUserInterest(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("password"))
            {
                _chatbotService.StoreUserInterest("password safety");
                AddBotMessage("Great! I'll remember that you're interested in password safety. It's a crucial part of staying safe online!", "Memory");
            }
            else if (lowerInput.Contains("phish") || lowerInput.Contains("scam"))
            {
                _chatbotService.StoreUserInterest("phishing protection");
                AddBotMessage("Excellent! I'll remember you want to learn about phishing protection. Scammers are getting smarter, but so are you!", "Memory");
            }
            else if (lowerInput.Contains("brows") || lowerInput.Contains("wifi"))
            {
                _chatbotService.StoreUserInterest("safe browsing");
                AddBotMessage("Good choice! I'll note that you're interested in safe browsing habits. The web can be safer with the right knowledge!", "Memory");
            }
            else if (lowerInput.Contains("privacy"))
            {
                _chatbotService.StoreUserInterest("privacy");
                AddBotMessage("Privacy is so important! I'll remember you want to learn about protecting your personal information online.", "Memory");
            }
        }

        private string RecallInformation(string userInput)
        {
            string lowerInput = userInput.ToLower();

            string interests = _chatbotService.GetUserInterests();
            if (!string.IsNullOrEmpty(interests) && (lowerInput.Contains("interest") || lowerInput.Contains("like")))
            {
                return $"I remember you're interested in {interests}. Would you like me to share more tips about these topics?";
            }

            foreach (var topic in _conversationMemory.Keys)
            {
                if (lowerInput.Contains(topic.ToLower()))
                {
                    return $"I recall you were asking about {topic} earlier. Would you like me to share more {topic} tips with you?";
                }
            }

            if (_conversationMemory.Count > 0 && !string.IsNullOrEmpty(_currentTopic))
            {
                string userName = _chatbotService.GetUserName();
                if (!string.IsNullOrEmpty(userName))
                {
                    return $"{userName}, we were discussing {_currentTopic} before. Would you like me to continue with that topic?";
                }
                return $"We were discussing {_currentTopic} earlier. Would you like more information about this?";
            }

            return null;
        }

        private void StoreConversationHistory(string userInput, string botResponse)
        {
            if (!string.IsNullOrEmpty(_currentTopic) && _currentTopic != "General" && _currentTopic != "Invalid Input")
            {
                if (!_conversationMemory.ContainsKey(_currentTopic))
                {
                    _conversationMemory[_currentTopic] = new List<string>();
                }

                _conversationMemory[_currentTopic].Add($"User: {userInput}");
                _conversationMemory[_currentTopic].Add($"Bot: {botResponse}");

                if (_conversationMemory[_currentTopic].Count > 20)
                {
                    _conversationMemory[_currentTopic].RemoveRange(0, 10);
                }
            }
        }
    }
}