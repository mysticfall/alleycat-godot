using System.Collections.Generic;

namespace AlleyCat.Action
{
    public interface IActionSet : IReadOnlyDictionary<string, IAction>
    {
        IEnumerable<IActionGroup> Groups { get; }
    }
}
