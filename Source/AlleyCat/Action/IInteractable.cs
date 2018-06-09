using System.Collections.Generic;

namespace AlleyCat.Action
{
    public interface IInteractable
    {
        IEnumerable<IAction> Actions { get; }
    }
}
