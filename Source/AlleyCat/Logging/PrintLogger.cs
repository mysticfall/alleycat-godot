using System;
using System.Text;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [ProviderAlias("Editor")]
    public class PrintLogger : Logger
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public PrintLogger(
            string name,
            int categorySegments = 1,
            bool showId = true) : base(name, categorySegments, showId)
        {
        }

        public PrintLogger(
            string name,
            Option<IExternalScopeProvider> scopeProvider,
            int categorySegments = 1,
            bool showId = true) : base(name, scopeProvider, categorySegments, showId)
        {
        }

        protected override void Log(
            LogLevel logLevel, string message, Option<string> loggerId, Option<Exception> exception)
        {
            var level = FormatLogLevel(logLevel);

            _builder.Append(level);
            _builder.Append(" - [");
            _builder.Append(CategoryLabel);
            _builder.Append("] ");

            loggerId.Iter(id =>
            {
                _builder.Append("(");
                _builder.Append(id);
                _builder.Append(") ");
            });

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
