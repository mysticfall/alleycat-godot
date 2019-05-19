using Godot;

namespace AlleyCat.UI.Console
{
    public interface IConsole
    {
        Color TextColor { get; }

        Color HighlightColor { get; }

        Color WarningColor { get; }

        Color ErrorColor { get; }

        IConsole Write(string text, TextStyle style);

        IConsole NewLine();

        void Clear();
    }
}
