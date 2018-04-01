using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IConsole
    {
        int BufferSize { get; set; }

        Color TextColor { get; }

        Color HighlightColor { get; }

        Color WarningColor { get; }

        Color ErrorColor { get; }

        [NotNull]
        IConsole Write([NotNull] string text, TextStyle style);

        [NotNull]
        IConsole NewLine();

        void Clear();
    }
}
