using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Autowire;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    [Singleton(typeof(IConsoleCommandProvider))]
    public abstract class ConsoleCommandProvider : AutowiredNode, IConsoleCommandProvider
    {
        public IEnumerable<IConsoleCommand> Commands { get; private set; }

        protected ConsoleCommandProvider()
        {
            Commands = Enumerable.Empty<IConsoleCommand>();
        }

        public override void _Ready()
        {
            base._Ready();

            var commands = CreateCommands();

            Debug.Assert(commands != null, "CreateCommands() returned null.");

            Commands = commands;
        }

        [NotNull]
        protected abstract IEnumerable<IConsoleCommand> CreateCommands();
    }
}
