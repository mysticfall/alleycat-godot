using System;
using System.Collections.Generic;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public interface IMenu
    {
        Option<IMenuItem> Current { get; }

        IObservable<Option<IMenuItem>> OnNavigate { get; }

        IEnumerable<IMenuItem> Items { get; }

        IObservable<IEnumerable<IMenuItem>> OnItemsChange { get; }

        void Navigate(Option<IMenuItem> item);
    }

    public static class MenuExtensions
    {
        public static void ToTop(this IMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            menu.Navigate(None);
        }

        public static bool CanGoUp(this IMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            return menu.Current.IsSome;
        }

        public static void GoUp(this IMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            var value = menu.Current.Bind(m => m.Parent);

            menu.Navigate(value);
        }
    }
}
