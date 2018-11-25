using System;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using static LanguageExt.Prelude;

namespace AlleyCat.Logging
{
    public abstract class Logger : ILogger
    {
        public string Category { get; }

        public Option<string> LoggerId { get; }

        protected Option<IExternalScopeProvider> ScopeProvider { get; }
        protected string CategoryLabel { get; }

        protected bool ShowId { get; }

        private Lst<object> _scopes;

        protected Logger(
            string category,
            int categorySegments = 1,
            bool showId = true) : this(category, None, categorySegments, showId)
        {
        }

        protected Logger(
            string category,
            Option<IExternalScopeProvider> scopeProvider,
            int categorySegments = 1,
            bool showId = true)
        {
            Ensure.That(category, nameof(category)).IsNotNull();

            Category = category;
            ScopeProvider = scopeProvider;

            var segments = Category.Split(".").Reverse().Take(categorySegments).Freeze();

            var (label, id) = segments.Match(
                () => ("", None),
                head =>
                {
                    var values = head.Split(":");

                    return values.Length == 2 ? (values[0], Some(values[1])) : (head, None);
                },
                More: (head, tail) =>
                {
                    var values = head.Split(":");
                    var list = tail.Prepend(values.Length == 2 ? values[0] : head);

                    return (string.Join(".", list.Reverse()), values.Skip(1).HeadOrNone());
                });

            CategoryLabel = label;
            LoggerId = id;

            ShowId = showId;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            [CanBeNull] Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null) return;

            Log(logLevel, message, ShowId ? LoggerId | Optional(eventId.Name) : None, Optional(exception));
        }

        protected abstract void Log(
            LogLevel logLevel,
            string message,
            Option<string> eventId,
            Option<Exception> exception);

        public virtual bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        protected virtual string FormatLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "TRACE";
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Information:
                    return "INFO";
                case LogLevel.Warning:
                    return "WARN";
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Critical:
                    return "FATAL";
                case LogLevel.None:
                    return "NONE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public virtual IDisposable BeginScope<TState>(TState state) =>
            ScopeProvider.Map(p => p.Push(state)).IfNone(NullScope.Instance);
    }
}
