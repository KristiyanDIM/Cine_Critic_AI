using System;
using System.Collections.Generic;
using System.IO;

namespace Cine_Critic_AI.Services
{
    public sealed class AppLoggerSingleton
    {
        private static readonly Lazy<AppLoggerSingleton> lazy =
            new Lazy<AppLoggerSingleton>(() => new AppLoggerSingleton());

        public static AppLoggerSingleton Instance => lazy.Value;

        private readonly List<string> _logs = new List<string>();
        private readonly string _logFilePath;

        private AppLoggerSingleton()
        {
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppLogs.txt");
        }

        public void Log(string message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            _logs.Add(logEntry);

            // Опционално: запис в текстов файл
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }

        public IReadOnlyList<string> GetLogs() => _logs.AsReadOnly();
    }
}
