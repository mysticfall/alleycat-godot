using EnsureThat;

namespace AlleyCat.Common
{
    public class IdentifiableDirectory<T> : Directory<T> where T : IIdentifiable
    {
        protected override string GetKey(T item)
        {
            Ensure.Any.IsNotNull(item, nameof(item));

            return item.Key;
        }
    }
}
