using AlleyCat.Common;

namespace AlleyCat.UI.Console
{
    public interface IConsoleCommand : IIdentifiable, IDescribable
    {
        ICommandConsole Console { get; }

        void Execute(params string[] args);

        void DisplayUsage();
    }
}
