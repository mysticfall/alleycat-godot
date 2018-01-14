namespace AlleyCat.UI.Console
{
    public interface IConsoleCommand
    {
        string Name { get; }

        string Description { get; }

        void Execute(string[] args, IConsole console);
    }
}
