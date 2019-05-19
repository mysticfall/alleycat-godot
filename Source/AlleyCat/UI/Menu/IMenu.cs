using System;
using System.Collections.Generic;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public interface IMenu
    {
        Option<IMenuModel> Current { get; }

        IObservable<Option<IMenuModel>> OnNavigate { get; }

        IEnumerable<IMenuModel> Items { get; }

        IObservable<IEnumerable<IMenuModel>> OnItemsChange { get; }

        void Navigate(Option<IMenuModel> item);
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
