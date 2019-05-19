using AlleyCat.Autowire;
using EnsureThat;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IHideable
    {
        bool Visible { get; set; }
    }

    public static class HideableExtensions
    {
        public static void Show(this IHideable hideable)
        {
            Ensure.That(hideable, nameof(hideable)).IsNotNull();

            hideable.Visible = true;
        }

        public static void Hide(this IHideable hideable)
        {
            Ensure.That(hideable, nameof(hideable)).IsNotNull();

            hideable.Visible = false;
        }

        public static void ToggleVisibility(this IHideable hideable)
        {
            Ensure.That(hideable, nameof(hideable)).IsNotNull();

            hideable.Visible = !hideable.Visible;
        }
    }
}
