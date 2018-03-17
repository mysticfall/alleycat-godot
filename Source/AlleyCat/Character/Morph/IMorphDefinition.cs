using AlleyCat.Common;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public interface IMorphDefinition : INamed
    {
        [NotNull]
        IMorphGroup Group { get; }

        [NotNull]
        IMorph CreateMorph([NotNull] IMorphable morphable);
    }
}
