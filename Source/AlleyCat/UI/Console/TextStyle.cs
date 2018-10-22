using System.Text;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Console
{
    public struct TextStyle
    {
        public Option<Color> Color { get; }

        public bool Italics { get; }

        public bool Bold { get; }

        public bool Underline { get; }

        public TextStyle(
            bool italics = false,
            bool bold = false,
            bool underline = false) : this(None, italics, bold, underline)
        {
        }

        public TextStyle(
            Option<Color> color,
            bool italics = false,
            bool bold = false,
            bool underline = false)
        {
            Color = color;
            Italics = italics;
            Bold = bold;
            Underline = underline;
        }

        public TextStyle WithColor(Color color) => new TextStyle(color, Italics, Bold, Underline);

        public TextStyle WithoutColor() => new TextStyle(None, Italics, Bold, Underline);

        public TextStyle WithItalics() => new TextStyle(Color, true, Bold, Underline);

        public TextStyle WithoutItalics() => new TextStyle(Color, false, Bold, Underline);

        public TextStyle WithBold() => new TextStyle(Color, Italics, true, Underline);

        public TextStyle WithoutBold() => new TextStyle(Color, Italics, false, Underline);

        public TextStyle WithUnderline() => new TextStyle(Color, Italics, Bold, true);

        public TextStyle WithoutUnderline() => new TextStyle(Color, Italics, Bold);

        public void Write(string text, RichTextLabel label)
        {
            Ensure.That(text, nameof(text)).IsNotNull();
            Ensure.That(label, nameof(label)).IsNotNull();

            Color.Iter(label.PushColor);

            if (Underline) label.PushUnderline();

            if (Bold || Italics)
            {
                var sb = new StringBuilder(text.Length + 7 * 2);

                if (Bold) sb.Append("[b]");
                if (Italics) sb.Append("[i]");

                sb.Append(text);

                if (Italics) sb.Append("[/i]");
                if (Bold) sb.Append("[/b]");

                label.AppendBbcode(sb.ToString());
            }
            else
            {
                label.AddText(text);
            }

            if (Color.IsSome || Underline) label.Pop();
        }
    }
}
