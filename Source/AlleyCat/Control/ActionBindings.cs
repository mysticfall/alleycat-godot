using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Common;

namespace AlleyCat.Control
{
    public class ActionBindings : Directory<ActionBinding>
    {
        public IReadOnlyDictionary<string, IAction> ActionMap =>
            this.Select(i => i.Value.Action).ToDictionary(i => i.Key);
    }
}
