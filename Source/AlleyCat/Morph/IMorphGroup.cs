using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Game;

namespace AlleyCat.Morph
{
    public interface IMorphGroup : IGameResource, INamed, IEnumerable<IMorphDefinition>
    {
    }
}
