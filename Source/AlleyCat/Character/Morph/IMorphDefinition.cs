using AlleyCat.Common;

namespace AlleyCat.Character.Morph
{
    public interface IMorphDefinition : INamed
    {
        IMorphGroup Group { get; }

        IMorph CreateMorph(IMorphable morphable);
    }
}
