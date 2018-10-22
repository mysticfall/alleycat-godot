using AlleyCat.Condition.Generic;
using EnsureThat;
using Godot;

namespace AlleyCat.Condition
{
    public abstract class TypedCondition<T> : Node, ICondition<T>
    {
        public bool Matches(object context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            if (context != null && context is T type)
            {
                return Matches(type);
            }

            return false;
        }

        public abstract bool Matches(T context);
    }
}
