using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IConsole
    {
        int BufferSize { get; set; }

        Color TextColor { get; }

        Color HighlightColor { get; }

        Color WarningColor { get; }

        Color ErrorColor { get; }

        [NotNull]
        IEnumerable<IConsoleCommand> SupportedCommands { get; }

        void Open();

        void Close();

        void Toggle();

        [NotNull]
        IConsole Write([NotNull] string text);

        [NotNull]
        IConsole Write([NotNull] string text, TextStyle style);

        [NotNull]
        IConsole WriteLine([NotNull] string text);

        [NotNull]
        IConsole WriteLine([NotNull] string text, TextStyle style);

        [NotNull]
        IConsole NewLine();

        void Clear();

        void Execute([NotNull] string command, [CanBeNull] string[] arguments = null);
    }
}
