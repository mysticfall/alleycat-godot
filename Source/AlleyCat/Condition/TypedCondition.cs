using AlleyCat.Condition.Generic;
using Godot;

namespace AlleyCat.Condition
{
    public abstract class TypedCondition<T> : Node, ICondition<T>
    {
        public bool Matches(object context)
        {
            if (context != null && context is T type)
            {
                return Matches(type);
            }

            return false;
        }

        public abstract bool Matches(T context);
    }
}
