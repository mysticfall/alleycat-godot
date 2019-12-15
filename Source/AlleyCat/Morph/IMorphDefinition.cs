using AlleyCat.Common;
using AlleyCat.Game;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public interface IMorphDefinition : IGameResource, INamed
    {
        bool Hidden { get; }

        IMorph CreateMorph(IMorphable morphable, ILoggerFactory loggerFactory);
    }
}
