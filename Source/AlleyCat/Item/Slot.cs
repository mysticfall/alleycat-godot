using AlleyCat.Autowire;
using AlleyCat.Condition.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public abstract class Slot : AutowiredNode, ISlot
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        [Node(required: false), UsedImplicitly]
        private ICondition<ISlotItem> _allowedFor;

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;

        public virtual bool AllowedFor(ISlotItem context)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            return _allowedFor == null || _allowedFor.Matches(context);
        }

        public bool AllowedFor(object context) => context is ISlotItem item && AllowedFor(item);
    }
}
