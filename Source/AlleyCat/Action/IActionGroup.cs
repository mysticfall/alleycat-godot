using System.Collections.Generic;
using AlleyCat.Common;

namespace AlleyCat.Action
{
    public interface IActionGroup : INamed
    {
        IEnumerable<IActionGroup> Groups { get; }

        IEnumerable<IAction> Actions { get; }
    }
}
