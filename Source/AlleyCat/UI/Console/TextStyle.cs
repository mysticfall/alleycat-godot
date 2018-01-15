using System.Text;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public struct TextStyle
    {
        [CanBeNull]
        public Color? Color { get; }

        public bool Italics { get; }

        public bool Bold { get; }

        public bool Underline { get; }

        public TextStyle(
            [CanBeNull] Color? color = null,
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

        public TextStyle WithoutColor() => new TextStyle(null, Italics, Bold, Underline);

        public TextStyle WithItalics() => new TextStyle(Color, true, Bold, Underline);

        public TextStyle WithoutItalics() => new TextStyle(Color, false, Bold, Underline);

        public TextStyle WithBold() => new TextStyle(Color, Italics, true, Underline);

        public TextStyle WithoutBold() => new TextStyle(Color, Italics, false, Underline);

        public TextStyle WithUnderline() => new TextStyle(Color, Italics, Bold, true);

        public TextStyle WithoutUnderline() => new TextStyle(Color, Italics, Bold, false);

        public void Write([NotNull] string text, [NotNull] RichTextLabel label)
        {
            Ensure.Any.IsNotNull(text, nameof(text));
            Ensure.Any.IsNotNull(label, nameof(label));

            if (Color.HasValue) label.PushColor(Color.Value);
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

            if (Color.HasValue || Underline) label.Pop();
        }
    }
}
