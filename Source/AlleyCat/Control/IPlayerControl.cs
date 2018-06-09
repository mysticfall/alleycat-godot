using System.Collections.Generic;
using AlleyCat.Action;
using AlleyCat.View;

namespace AlleyCat.Control
{
    public interface IPlayerControl : IPerspectiveSwitcher
    {
        IReadOnlyDictionary<string, IAction> Actions { get; }
    }
}
