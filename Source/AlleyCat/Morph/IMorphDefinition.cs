using AlleyCat.Common;

namespace AlleyCat.Morph
{
    public interface IMorphDefinition : INamed
    {
        bool Hidden { get; }

        IMorph CreateMorph(IMorphable morphable);
    }
}
