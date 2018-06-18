using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
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

        public string Slot => _slot;

        public IEnumerable<string> AdditionalSlots => _additionalSlots.TrimToEnumerable();

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;

        [Export, UsedImplicitly] private string _slot;

        [Export, UsedImplicitly] private string _additionalSlots;

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
