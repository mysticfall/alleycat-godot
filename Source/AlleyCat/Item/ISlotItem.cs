using AlleyCat.Condition.Generic;
using Godot;

namespace AlleyCat.Item
{
    public interface ISlotItem : IItem, ISlotConfiguration, IRestricted<ISlotContainer>
    {
        Node Node { get; }
    }

    namespace Generic
    {
        public interface ISlotItem<out T> : ISlotItem where T : Node
        {
            new T Node { get; }
        }
    }
}
