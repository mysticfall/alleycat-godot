using AlleyCat.Common;

namespace AlleyCat.Morph
{
    public interface IMorphDefinition : INamed
    {
        IMorph CreateMorph(IMorphable morphable);
    }
}
