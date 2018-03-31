using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IConsoleCommandProvider
    {
        [NotNull]
        IEnumerable<IConsoleCommand> CreateCommands([NotNull] ICommandConsole console);
    }
}
