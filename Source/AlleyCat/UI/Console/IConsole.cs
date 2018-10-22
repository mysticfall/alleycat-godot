using Godot;

namespace AlleyCat.UI.Console
{
    public interface IConsole
    {
        int BufferSize { get; set; }

        Color TextColor { get; }

        Color HighlightColor { get; }

        Color WarningColor { get; }

        Color ErrorColor { get; }

        IConsole Write(string text, TextStyle style);

        IConsole NewLine();

        void Clear();
    }
}
