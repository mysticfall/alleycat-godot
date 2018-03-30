using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface ICommandConsole : IConsole
    {
        [NotNull]
        IEnumerable<IConsoleCommand> SupportedCommands { get; }

        void Execute([NotNull] string command, [CanBeNull] string[] arguments = null);
    }
}
