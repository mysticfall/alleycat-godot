using System;
using System.Text;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    public class PrintLogger : Logger
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public PrintLogger(
            string category,
            LogLevel minimumLevel = LogLevel.Trace,
            int categorySegments = 1,
            bool showId = true) : base(category, minimumLevel, categorySegments, showId)
        {
        }

        public PrintLogger(
            string category,
            Option<IExternalScopeProvider> scopeProvider,
            LogLevel minimumLevel = LogLevel.Trace,
            int categorySegments = 1,
            bool showId = true) : base(category, scopeProvider, minimumLevel, categorySegments, showId)
        {
        }

        protected override void Log(
            LogLevel logLevel, string message, Option<string> eventId, Option<Exception> exception)
        {
            var level = FormatLogLevel(logLevel);

            _builder.Append(level);
            _builder.Append(" - [");
            _builder.Append(CategoryLabel);

            eventId.Iter(id =>
            {
                _builder.Append(" (");
                _builder.Append(id);
                _builder.Append(")");
            });

            _builder.Append("] ");

            _builder.Append(message);

            exception.Iter(e =>
            {
                _builder.AppendLine();
                _builder.Append(e.ToString());
            });

            GD.Print(_builder.ToString());

            _builder.Clear();
        }
    }
}
