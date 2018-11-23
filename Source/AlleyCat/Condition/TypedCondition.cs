using AlleyCat.Condition.Generic;
using Godot;

namespace AlleyCat.Condition
{
    public abstract class TypedCondition<T> : Node, ICondition<T>
    {
        public bool Matches(object context) => context is T type && Matches(type);

        public abstract bool Matches(T context);
    }
}
