using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public static class ConsoleExtensions
    {
        [NotNull]
        public static IConsole Text([NotNull] this IConsole console, [NotNull] string text)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            return console.Write(text, new TextStyle(console.TextColor));
        }

        [NotNull]
        public static IConsole Highlight([NotNull] this IConsole console, [NotNull] string text)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            return console.Write(text, new TextStyle(console.HighlightColor));
        }

        [NotNull]
        public static IConsole Warning([NotNull] this IConsole console, [NotNull] string text)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            return console.Write(text, new TextStyle(console.WarningColor));
        }

        [NotNull]
        public static IConsole Error([NotNull] this IConsole console, [NotNull] string text)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            return console.Write(text, new TextStyle(console.ErrorColor));
        }
    }
}
