using System.Collections.Generic;
using AlleyCat.Interaction;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IConsole : IHideable
    {
        Color TextColor { get; }

        Color HighlightColor { get; }

        Color WarningColor { get; }

        Color ErrorColor { get; }

        [NotNull]
        IEnumerable<IConsoleCommand> SupportedCommands { get; }

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
