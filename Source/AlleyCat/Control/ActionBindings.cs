using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Control
{
    [AutowireContext]
    public class ActionBindings : IdentifiableDirectory<ActionBinding>
    {
        public IReadOnlyDictionary<string, IAction> ActionMap =>
            this.Select(i => i.Value.Action).ToDictionary(i => i.Key);
    }
}
