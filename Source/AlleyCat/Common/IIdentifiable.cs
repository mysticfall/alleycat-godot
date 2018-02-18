using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IIdentifiable
    {
        [NotNull]
        string Key { get; }
    }
}
