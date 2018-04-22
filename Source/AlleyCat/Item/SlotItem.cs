using AlleyCat.Autowire;
using AlleyCat.Condition.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public abstract class SlotItem : AutowiredNode, ISlotItem
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        [Export, UsedImplicitly]
        public string Slot { get; private set; }

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;

        [Node(required: false), UsedImplicitly]
        private ICondition<ISlotContainer> _allowedFor;

        public virtual bool AllowedFor(ISlotContainer context)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            return _allowedFor == null || _allowedFor.Matches(context);
        }

        public bool AllowedFor(object context) => context is ISlotContainer container && AllowedFor(container);
    }
}
