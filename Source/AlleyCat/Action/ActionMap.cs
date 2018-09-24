using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Action
{
    [Singleton(typeof(ActionMap), typeof(IReadOnlyDictionary<string, IAction>))]
    public class ActionMap : IdentifiableDirectory<IAction>
    {
    }
}
