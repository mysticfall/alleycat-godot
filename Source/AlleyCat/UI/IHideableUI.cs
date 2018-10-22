using AlleyCat.Common;

namespace AlleyCat.UI
{
    public interface IHideableUI : IHideable
    {
        void Show();

        void Hide();
    }

    public static class HideableUIExtensions
    {
        public static void Toggle(this IHideableUI ui)
        {
            if (ui.Visible)
            {
                ui.Hide();
            }
            else
            {
                ui.Show();
            }
        }
    }
}
