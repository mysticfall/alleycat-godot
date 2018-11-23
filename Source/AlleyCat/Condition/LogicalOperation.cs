using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AlleyCat.Autowire;

namespace AlleyCat.Condition
{
    public class LogicalOperation : AutowiredNode, ICondition
    {
        [Export]
        public LogicalOperationType Type { get; set; } = LogicalOperationType.All;

        [Node(".")]
        public IEnumerable<ICondition> Conditions { get; private set; }

        public bool Matches(object context)
        {
            switch (Type)
            {
                case LogicalOperationType.All:
                    return Conditions.All(c => c.Matches(context));
                case LogicalOperationType.Any:
                    return Conditions.Any(c => c.Matches(context));
                case LogicalOperationType.Not:
                    return Conditions.FirstOrDefault()?.Matches(context) ?? false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
