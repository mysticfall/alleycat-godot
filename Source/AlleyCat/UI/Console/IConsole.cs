using System.Collections.Generic;
using AlleyCat.Interaction;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IConsole : IHideable
    {
        [NotNull]
        IEnumerable<IConsoleCommand> SupportedCommands { get; }

        [NotNull]
        IConsole Write([NotNull] string text);

        [NotNull]
        IConsole WriteLine([NotNull] string text);

        [NotNull]
        IConsole NewLine();

        void Clear();

        void Execute([NotNull] string command, [CanBeNull] string[] arguments = null);
    }
}
