using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IConsoleCommand
    {
        [NotNull]
        string Key { get; }

        [NotNull]
        string Description { get; }

        void Execute([CanBeNull] string[] args, [NotNull] IConsole console);

        void DisplayUsage([NotNull] IConsole console);
    }
}
