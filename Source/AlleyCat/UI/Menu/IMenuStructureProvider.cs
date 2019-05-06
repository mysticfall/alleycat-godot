using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.UI.Menu
{
    public interface IMenuStructureProvider
    {
        bool HasChildren(object item);

        [CanBeNull] IEnumerable<object> FindChildren(object item);
    }
}
