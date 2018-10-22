using System;
using AlleyCat.UI.Console;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    public class ConsoleLogger : ILogger
    {
        public string Name { get; }

        public DebugConsole Console { get; }

        public ConsoleLogger(string name, DebugConsole console)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(console, nameof(console)).IsNotNull();

            Name = name;
            Console = console;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null) return;

            var prefix = GetLevelPrefix(logLevel);

            Color color;

            switch (logLevel)
            {
                case LogLevel.Warning:
                    color = Console.WarningColor;
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    color = Console.ErrorColor;
                    break;
                default:
                    color = Console.TextColor;
                    break;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            Console
                .Highlight("[").Write(prefix, new TextStyle(color)).Highlight("]")
                .Highlight("[").Text(Name).Highlight("] ")
                .Text(message)
                .NewLine();
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        protected virtual string GetLevelPrefix(LogLevel level)
        {
            var prefix = level.ToString();

            return prefix.Length < 6 ? prefix : prefix.Left(4);
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
