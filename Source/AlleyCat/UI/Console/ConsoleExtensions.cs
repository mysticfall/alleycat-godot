using EnsureThat;

namespace AlleyCat.UI.Console
{
    public static class ConsoleExtensions
    {
        public static IConsole Text(this IConsole console, string text)
        {
            Ensure.That(console, nameof(console)).IsNotNull();
            Ensure.That(text, nameof(text)).IsNotNull();

            return console.Write(text, new TextStyle(console.TextColor));
        }

        public static IConsole Highlight(this IConsole console, string text)
        {
            Ensure.That(console, nameof(console)).IsNotNull();

            return console.Write(text, new TextStyle(console.HighlightColor));
        }

        public static IConsole Warning(this IConsole console, string text)
        {
            Ensure.That(console, nameof(console)).IsNotNull();

            return console.Write(text, new TextStyle(console.WarningColor));
        }

        public static IConsole Error(this IConsole console, string text)
        {
            Ensure.That(console, nameof(console)).IsNotNull();

            return console.Write(text, new TextStyle(console.ErrorColor));
        }
    }
}
