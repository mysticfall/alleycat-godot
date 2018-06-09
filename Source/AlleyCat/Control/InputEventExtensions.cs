using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public static class InputEventExtensions
    {
        [CanBeNull]
        public static string GetKeyLabel([NotNull] this InputEvent @event)
        {
            Ensure.Any.IsNotNull(@event, nameof(@event));

            // TODO Handle special keys, and other input devices like joypads.
            switch (@event)
            {
                case InputEventKey key:
                    return ((char) key.Scancode).ToString();
                default:
                    return null;
            }
        }
    }
}
