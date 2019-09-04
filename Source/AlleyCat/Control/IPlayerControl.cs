using System.Collections.Generic;
using AlleyCat.Character;
using AlleyCat.View;

namespace AlleyCat.Control
{
    public interface IPlayerControl : ICharacterAware<IHumanoid>, IPerspectiveSwitcher
    {
        IEnumerable<IInput> Inputs { get; }
    }
}
