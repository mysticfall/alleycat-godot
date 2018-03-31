using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IConsoleCommand
    {
        [NotNull]
        string Key { get; }

        [NotNull]
        string Description { get; }

        ICommandConsole Console { get; }

        void Execute([CanBeNull] string[] args);

        void DisplayUsage();
    }
}
