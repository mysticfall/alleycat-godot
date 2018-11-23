using System.Linq;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public static class InputEventExtensions
    {
        public static Option<string> FindKeyLabel(this InputEvent @event)
        {
            // TODO Handle special keys, and other input devices like joypads.
            return Optional(@event)
                .OfType<InputEventKey>()
                .Map(e => (char) e.Scancode)
                .Map(c => c.ToString())
                .HeadOrNone();
        }
    }
}
