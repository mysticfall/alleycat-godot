using AlleyCat.Common;

namespace AlleyCat.Character.Morph
{
    public interface IMorphDefinition : INamed
    {
        IMorph CreateMorph(IMorphable morphable);
    }
}
