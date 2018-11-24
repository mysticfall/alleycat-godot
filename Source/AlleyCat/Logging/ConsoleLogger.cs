using System;
using AlleyCat.UI.Console;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Logging
{
    [ProviderAlias("Console")]
    public class ConsoleLogger : Logger
    {
        public IConsole Console { get; }

        public ConsoleLogger(
            string name,
            IConsole console,
            int categorySegments = 1,
            bool showId = true) : this(name, console, None, categorySegments, showId)
        {
        }

        public ConsoleLogger(
            string name,
            IConsole console,
            Option<IExternalScopeProvider> scopeProvider,
            int categorySegments = 1,
            bool showId = true) : base(name, scopeProvider, categorySegments, showId)
        {
            Ensure.That(console, nameof(console)).IsNotNull();

            Console = console;
        }

        protected override void Log(
            LogLevel logLevel, string message, Option<string> loggerId, Option<Exception> exception)
        {
            var level = FormatLogLevel(logLevel);

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

            Console
                .Write(level, new TextStyle(color)).Text(" - ")
                .Highlight("[").Text(CategoryLabel).Highlight("] ");

            loggerId.Iter(id => Console.Highlight("(").Text(id).Highlight(") "));

            Console.Text(message).NewLine();

            exception.Iter(e =>
            {
                var style = new TextStyle(Console.ErrorColor);

                Console
                    .Write(e.ToString(), style)
                    .NewLine()
                    .Write(e.StackTrace, style)
                    .NewLine();
            });
        }
    }
}
