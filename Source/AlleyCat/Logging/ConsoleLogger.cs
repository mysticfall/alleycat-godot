using System;
using AlleyCat.UI.Console;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Logging
{
    public class ConsoleLogger : Logger
    {
        public Func<Option<IConsole>> Console { get; }

        public ConsoleLogger(
            string category,
            Func<Option<IConsole>> console,
            LogLevel minimumLevel = LogLevel.Trace,
            int categorySegments = 1,
            bool showId = true) : this(category, console, None, minimumLevel, categorySegments, showId)
        {
        }

        public ConsoleLogger(
            string category,
            Func<Option<IConsole>> console,
            Option<IExternalScopeProvider> scopeProvider,
            LogLevel minimumLevel = LogLevel.Trace,
            int categorySegments = 1,
            bool showId = true) : base(category, scopeProvider, minimumLevel, categorySegments, showId)
        {
            Ensure.That(console, nameof(console)).IsNotNull();

            Console = console;
        }

        protected override void Log(
            LogLevel logLevel, string message, Option<string> eventId, Option<Exception> exception)
        {
            Console().Iter(console => Log(console, logLevel, message, eventId, exception));
        }

        protected void Log(
            IConsole console, LogLevel logLevel, string message, Option<string> eventId, Option<Exception> exception)
        {
            var level = FormatLogLevel(logLevel);

            Color color;

            switch (logLevel)
            {
                case LogLevel.Warning:
                    color = console.WarningColor;
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    color = console.ErrorColor;
                    break;
                default:
                    color = console.TextColor;
                    break;
            }

            console
                .Write(level, new TextStyle(color)).Text(" - ")
                .Highlight("[").Text(CategoryLabel);

            eventId.Iter(id => console.Highlight(" (").Text(id).Highlight(")"));

            console.Highlight("] ").Text(message).NewLine();

            exception.Iter(e =>
            {
                console
                    .Write(e.ToString(), new TextStyle(color))
                    .NewLine();
            });
        }
    }
}
