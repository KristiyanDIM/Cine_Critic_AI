using System;
using System.Collections.Generic;
using System.IO;

namespace Cine_Critic_AI.Services
{
    // Singleton Logger клас
    // Гарантира, че има само един екземпляр на AppLoggerSingleton за цялото приложение
    public sealed class AppLoggerSingleton
    {
        // Lazy инициализация на Singleton-а
        // Обектът ще се създаде само при първото извикване на Instance
        private static readonly Lazy<AppLoggerSingleton> lazy =
            new Lazy<AppLoggerSingleton>(() => new AppLoggerSingleton());

        // Публичен достъп до Singleton екземпляра
        public static AppLoggerSingleton Instance => lazy.Value;

        // Списък за съхранение на логове в паметта
        private readonly List<string> _logs = new List<string>();

        // Път към файла, в който ще се записват логовете
        private readonly string _logFilePath;

        // Приватен конструктор – предотвратява създаването на други екземпляри
        private AppLoggerSingleton()
        {
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppLogs.txt");
        }

        // Метод за записване на лог съобщение
        public void Log(string message)
        {
            // Форматиране на лог съобщението с дата и час
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            // Добавяне в списъка с логове
            // В текущата реализация _logs не е thread-safe при многопоточен достъп
            _logs.Add(logEntry);

            // Записване на лог в текстов файл
            // Може да се добави ротация на файла, за да не стане твърде голям
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }

        // Метод за достъп до логовете като read-only списък
        public IReadOnlyList<string> GetLogs() => _logs.AsReadOnly();
    }
}
