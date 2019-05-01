using System.Collections.Generic;

namespace AlleyCat.Action
{
    public interface IActionSet : IReadOnlyDictionary<string, IAction>
    {
        IEnumerable<IAction> Actions { get; }

        IEnumerable<IActionGroup> Groups { get; }
    }
}
