using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public interface IMenuModel : INamed
    {
        object Model { get; }

        Option<IMenuModel> Parent { get; }
    }

    public static class MenuItemExtensions
    {
        public static IEnumerable<IMenuModel> GetPath(this IMenuModel item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            yield return item;

            var parent = Some(item);

            while ((parent = parent.Bind(p => p.Parent)).IsSome)
            {
                yield return parent.Head();
            }
        }
    }
}
