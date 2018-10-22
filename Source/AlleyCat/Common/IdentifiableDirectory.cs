using System.Diagnostics;
using EnsureThat;

namespace AlleyCat.Common
{
    public class IdentifiableDirectory<T> : Directory<T> where T : class, IIdentifiable
    {
        protected override string GetKey(T item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            Debug.Assert(item.Key != null, "item.Key != null");

            return item.Key;
        }
    }
}
