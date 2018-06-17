using System.Collections.Generic;
using System.Linq;
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

        public string Slot => _slot;

        public IEnumerable<string> AdditionalSlots =>
            _additionalSlots?.Split(",").Select(v => v.Trim()).Where(v => v != "Null") ?? Enumerable.Empty<string>();

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
