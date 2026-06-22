# Cybersecurity Awareness Bot - Part 3 (POE)

A comprehensive WPF desktop application that educates South African citizens about cybersecurity through an interactive chatbot with task management, quizzes, and activity logging.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Database Setup](#database-setup)
- [How to Use](#how-to-use)
- [Commands Reference](#commands-reference)
- [Releases](#releases)

---

## Features

### Part 1 Features (Console Version)
- **Voice Greeting** - Plays a welcome message when the application starts
- **ASCII Art Logo** - Displays a cybersecurity-themed logo
- **Text-Based Greeting** - Personalized welcome with user's name
- **Basic Response System** - Answers questions about cybersecurity topics
- **Input Validation** - Handles invalid inputs gracefully
- **Enhanced Console UI** - Colored text, borders, and visual elements

### Part 2 Features (GUI Version)
- **GUI Interface** - Clean Windows Presentation Foundation (WPF) design with chat bubbles
- **Keyword Recognition** - Recognizes topics like password, phishing, browsing, privacy
- **Random Responses** - Gives varied answers so conversations feel natural
- **Memory & Recall** - Remembers your name and topics you're interested in
- **Sentiment Detection** - Detects if you're worried, confused, curious, or happy
- **Follow-up Questions** - Handles "tell me more" and "another tip" requests
- **Conversation Flow** - Maintains natural conversation without restarting
- **Error Handling** - Handles invalid inputs gracefully

### Part 3 Features (POE - New!)
- **Task Assistant with Reminders** - Add, view, complete, and delete cybersecurity tasks
- **Database Integration** - SQLite database for persistent task storage
- **Cybersecurity Mini-Game (Quiz)** - 12+ questions with immediate feedback and scoring
- **NLP Simulation** - Natural language understanding for commands like "add task", "remind me"
- **Activity Log Feature** - Tracks all user and bot actions with timestamps
- **Interactive Buttons** - One-click access to tasks, quiz, activity log, and help
- **Full GUI Integration** - All features accessible through the chat interface

### Example Questions and Commands:

| Command | Description |
|---------|-------------|
| `add task Review my privacy settings` | Adds a cybersecurity task |
| `remind me to update password tomorrow` | Sets a reminder for a task |
| `view tasks` or `tasks` | Shows all pending tasks |
| `quiz` or `game` | Starts the cybersecurity quiz |
| `activity log` or `log` | Shows recent activities |
| `complete [task name]` | Marks a task as complete |
| `delete [task name]` | Deletes a task |
| `help` | Shows available commands |
| `How do I create a strong password?` | Cybersecurity tip |
| `What is phishing?` | Cybersecurity tip |
| `I'm worried about scams` | Sentiment-based response |

---

## Technologies Used

| Technology | Purpose |
|------------|---------|
| C# .NET | Main programming language |
| WPF | Desktop GUI framework |
| XAML | User interface design |
| SQLite | Local database for task storage |
| System.Data.SQLite | SQLite database provider |
| System.Media | Audio playback for voice greeting |
| .NET Collections | Dictionaries, Lists for storing responses |

---

## Project Structure

```
CybersecurityChatbotWPF/
├── MainWindow.xaml              # GUI design (colors, layout, chat bubbles)
├── MainWindow.xaml.cs           # Main logic (message handling, sentiment)
├── ChatbotService.cs            # Chatbot brain (keywords, responses, NLP)
├── AudioService.cs              # Audio playback for voice greeting
├── DatabaseHelper.cs            # SQLite database operations (CRUD)
├── QuizService.cs               # Quiz questions, scoring, feedback
├── ActivityLog.cs               # Activity logging with timestamps
├── Data/
│   └── greeting.wav             # Voice greeting audio file
├── CybersecurityTasks.db        # SQLite database (auto-created)
├── packages.config              # NuGet package dependencies
└── README.md                    # This file
```

### File Descriptions:

| File | What it does |
|------|--------------|
| **MainWindow.xaml** | Creates the visual interface - colors, chat bubbles, input box, feature buttons |
| **MainWindow.xaml.cs** | Controls everything - sends messages, shows responses, handles tasks, quiz, activity log |
| **ChatbotService.cs** | Stores all knowledge - keywords, responses, NLP command parsing |
| **AudioService.cs** | Plays the voice greeting when app starts |
| **DatabaseHelper.cs** | SQLite database operations - add, view, complete, delete tasks |
| **QuizService.cs** | Manages quiz questions, checks answers, calculates scores |
| **ActivityLog.cs** | Tracks user and bot actions with timestamps |

---

## Setup Instructions

### Prerequisites

- Visual Studio 2022 or later
- .NET Framework 4.8 or later
- Windows OS

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/cybersecurity-chatbot.git
   ```

2. **Open the project**
   - Open Visual Studio
   - Click "Open a project or solution"
   - Navigate to the folder and select the `.csproj` file

3. **Install NuGet Packages**
   - Right-click the project → Manage NuGet Packages
   - Install the following packages:
     ```
     System.Data.SQLite
     System.Data.SQLite.Core
     ```
   - Or use Package Manager Console:
     ```powershell
     Install-Package System.Data.SQLite
     Install-Package System.Data.SQLite.Core
     ```

4. **Build the project**
   - Press `Ctrl + Shift + B` or click Build → Build Solution

5. **Run the application**
   - Press `F5` or click Debug → Start Debugging

### Database Setup

The SQLite database is automatically created when you first run the application. No manual setup is required.

**Database Location:**
- The database file (`CybersecurityTasks.db`) is created in your project's `bin\Debug` or `bin\Release` folder

**Database Schema:**
```sql
CREATE TABLE Tasks (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Description TEXT,
    ReminderDate TEXT,
    IsCompleted INTEGER DEFAULT 0,
    CreatedDate TEXT NOT NULL
)
```

### Audio Setup

For the voice greeting to work:

1. Place your `greeting.wav` file in the `Data` folder
2. In Solution Explorer, right-click the file → Properties
3. Set:
   - **Build Action**: `Content`
   - **Copy to Output Directory**: `Copy if newer`

If no audio file is found, the app will still work fine - just without the voice greeting.

---

## How to Use

### Starting the Chatbot

1. Run the application
2. The ASCII logo and welcome message will appear
3. Enter your name when prompted
4. Start chatting about cybersecurity!

### Sample Conversation - Adding a Task

```
Bot: Hello! Welcome to the Cybersecurity Awareness Bot!
Bot: What's your name?

User: John

Bot: Nice to meet you, John! I'm your Cybersecurity Awareness Assistant.
Bot: I can help with passwords, phishing, safe browsing, and privacy.

User: add task Review my privacy settings

Bot: Task ready: 'Review my privacy settings'. Would you like to set a reminder? (say 'yes' or 'no')

User: yes

Bot: Task added: 'Review my privacy settings' with reminder set for 2026-06-25.
```

### Sample Conversation - Taking the Quiz

```
User: quiz

Bot: Starting Cybersecurity Quiz! Answer each question to test your knowledge.

Bot: Question 1/12: Password
     What is the recommended minimum length for a strong password?
     A. 6 characters
     B. 8 characters
     C. 12 characters
     D. 16 characters

User: [Clicks option C]

Bot: Correct! Security experts recommend at least 12 characters for strong passwords.

... continues for all questions ...

Bot: Quiz Complete! Your score: 10/12
Bot: Great job! You have strong cybersecurity awareness! Keep learning!
```

### Sample Conversation - Viewing Tasks

```
User: view tasks

Bot: 📋 Your Tasks
     ✅ Review my privacy settings - ⏳ Pending (Reminder: 2026-06-25) [Done] [Delete]
     ✅ Enable two-factor authentication - ⏳ Pending [Done] [Delete]
```

### Sample Conversation - Activity Log

```
User: activity log

Bot: Recent Activities:
     1. 14:30 - Quiz: Completed with score 10/12
     2. 14:25 - Task: Added task: Review my privacy settings (Reminder: 2026-06-25)
     3. 14:20 - Quiz: Quiz started
     4. 14:15 - User: User registered: John
     5. 14:15 - Application: Cybersecurity Awareness Bot started
```

---

## Commands Reference

### Task Commands

| Command | Description | Example |
|---------|-------------|---------|
| `add task [description]` | Adds a new task | `add task Review my privacy settings` |
| `new task [description]` | Adds a new task | `new task Enable 2FA` |
| `remind me [task] in [X days]` | Sets a reminder | `remind me to update password tomorrow` |
| `view tasks` or `tasks` | Shows all pending tasks | `view tasks` |
| `complete [task name]` | Marks a task as complete | `complete Review privacy` |
| `delete [task name]` | Deletes a task | `delete Review privacy` |

### Quiz Commands

| Command | Description |
|---------|-------------|
| `quiz` or `game` | Starts the cybersecurity quiz |
| `start quiz` | Starts the cybersecurity quiz |

### Activity Commands

| Command | Description |
|---------|-------------|
| `activity log` or `log` | Shows recent activities |
| `what have you done` | Shows recent activities |
| `what did you do` | Shows recent activities |

### Other Commands

| Command | Description |
|---------|-------------|
| `help` or `menu` | Shows available commands and topics |
| `bye` or `exit` | Exits the application |

### Chat Topics

| Question | Topic |
|----------|-------|
| "How do I create a strong password?" | Password Safety |
| "What is phishing?" | Phishing Protection |
| "Is public Wi-Fi safe?" | Safe Browsing |
| "How can I protect my privacy?" | Privacy Tips |
| "Tell me another tip" | Follow-up Response |
| "I'm worried about scams" | Sentiment-based Response |



---

## Releases

| Release | Tag | Description |
|---------|-----|-------------|
| Part 1 | v1.0 | Console application with voice greeting and ASCII art |
| Part 2 | v2.0 | WPF GUI with keyword recognition, memory, and sentiment |
| Part 3 (POE) | v3.0 | Full application with tasks, quiz, NLP, and activity log |

---

## Troubleshooting

### Database Issues
- **Database not created**: Run the application once to create it automatically
- **Can't add tasks**: Check that SQLite NuGet packages are installed
- **Tasks not showing**: Verify database file exists in the output folder

### Audio Issues
- **No voice greeting**: Ensure `greeting.wav` is in the Data folder with correct build action
- **Audio error**: Check that the file is in WAV format

### Build Issues
- **NuGet package errors**: Restore packages via `dotnet restore` or Visual Studio Package Manager
- **C# version errors**: Ensure project is set to use C# 8.0 or higher

---

## Credits

- **Module**: Programming 2A (PROG6221)
- **Assessment**: Portfolio of Evidence (POE)
- **Year**: 2026

**Date**: June 2026
**Version**: 3.0 (POE)
