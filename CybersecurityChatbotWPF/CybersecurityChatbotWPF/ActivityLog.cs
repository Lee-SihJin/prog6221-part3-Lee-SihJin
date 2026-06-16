
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbotWPF
{
    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }

        public string DisplayText => $"{Timestamp:HH:mm} - {Action}: {Details}";

        public ActivityLogEntry(string action, string details)
        {
            Timestamp = DateTime.Now;
            Action = action;
            Details = details;
        }
    }

    public class ActivityLog
    {
        private List<ActivityLogEntry> _logEntries;
        private int _maxEntries;

        public ActivityLog(int maxEntries = 100)
        {
            _logEntries = new List<ActivityLogEntry>();
            _maxEntries = maxEntries;
        }

        public void AddEntry(string action, string details)
        {
            _logEntries.Add(new ActivityLogEntry(action, details));

            // Trim if exceeding max
            if (_logEntries.Count > _maxEntries)
            {
                _logEntries.RemoveRange(0, _logEntries.Count - _maxEntries);
            }
        }

        public List<ActivityLogEntry> GetEntries(int count = 10)
        {
            // Use reverse ordering with C# 7.3 compatible syntax
            var result = new List<ActivityLogEntry>();
            int startIndex = Math.Max(0, _logEntries.Count - count);
            for (int i = _logEntries.Count - 1; i >= startIndex; i--)
            {
                result.Add(_logEntries[i]);
            }
            return result;
        }

        public List<ActivityLogEntry> GetAllEntries()
        {
            var result = new List<ActivityLogEntry>();
            for (int i = _logEntries.Count - 1; i >= 0; i--)
            {
                result.Add(_logEntries[i]);
            }
            return result;
        }

        public string GetSummary(int count = 10)
        {
            var entries = GetEntries(count);
            if (entries.Count == 0)
                return "No recent activities.";

            string summary = "Recent Activities:\n\n";
            for (int i = 0; i < entries.Count; i++)
            {
                summary += $"{i + 1}. {entries[i].DisplayText}\n";
            }
            return summary;
        }

        public void Clear()
        {
            _logEntries.Clear();
        }

        public int Count => _logEntries.Count;
    }
}