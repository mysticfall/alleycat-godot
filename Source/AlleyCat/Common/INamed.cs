using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface INamed : IIdentifiable
    {
        [NotNull]
        string DisplayName { get; }
    }
}
