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
        public string Name { get; }

        protected Option<IExternalScopeProvider> ScopeProvider { get; }
        protected string CategoryLabel { get; }

        protected bool ShowId { get; }

        private Lst<object> _scopes;

        protected Logger(
            string name,
            int categorySegments = 1,
            bool showId = true) : this(name, None, categorySegments, showId)
        {
        }

        protected Logger(
            string name,
            Option<IExternalScopeProvider> scopeProvider,
            int categorySegments = 1,
            bool showId = true)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            Name = name;
            ScopeProvider = scopeProvider;

            var segments = Name.Split(".");

            CategoryLabel = string.Join(".", segments.Reverse().Take(categorySegments).Reverse());
            ShowId = showId;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            [CanBeNull] Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null) return;

            Log(logLevel, message, ShowId ? Optional(eventId.Name) : None, Optional(exception));
        }

        protected abstract void Log(
            LogLevel logLevel,
            string message,
            Option<string> loggerId,
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
