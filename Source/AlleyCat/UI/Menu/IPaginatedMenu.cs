using System;
using EnsureThat;

namespace AlleyCat.UI.Menu
{
    public interface IPaginatedMenu : IMenu
    {
        int Page { get; set; }

        int PageSize { get; }

        int ItemCount { get; }

        IObservable<int> OnPageChange { get; }
    }

    public static class PaginatedExtensions
    {
        public static void First(this IPaginatedMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            menu.Page = 0;
        }

        public static void Last(this IPaginatedMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            var lastPage = menu.PageSize == 0
                ? 0
                : (int) Math.Floor(Math.Abs(menu.ItemCount - 0.5f) / menu.PageSize);

            menu.Page = lastPage;
        }

        public static void Previous(this IPaginatedMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            menu.Page = Math.Max(0, menu.Page - 1);
        }

        public static void Next(this IPaginatedMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            var lastPage = menu.PageSize == 0
                ? 0
                : (int) Math.Floor(Math.Abs(menu.ItemCount - 0.5f) / menu.PageSize);

            menu.Page = Math.Min(lastPage, menu.Page + 1);
        }

        public static int PageCount(this IPaginatedMenu menu)
        {
            Ensure.That(menu, nameof(menu)).IsNotNull();

            var size = menu.PageSize;

            return size == 0 ? 0 : Math.Max(0, menu.ItemCount - 1) / menu.PageSize + 1;
        }
    }
}
