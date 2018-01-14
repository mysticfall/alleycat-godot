namespace AlleyCat.UI.Console
{
    public class HelpCommand : IConsoleCommand
    {
        public const string Key = "help";

        public string Name => Key;

        public string Description { get; } = "Show help information of console commands.";

        public void Execute(string[] args, IConsole console)
        {
            console
                .WriteLine("[Usage]")
                .WriteLine("help")
                .WriteLine("> Show this instruction.")
                .WriteLine("help list")
                .WriteLine("> Display list of available commands.")
                .WriteLine("help <command>")
                .WriteLine("> Display help message for the specified command.");
        }
    }
}
