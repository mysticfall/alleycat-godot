using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Condition.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class Slot : AutowiredNode, ISlot
    {
        public string Key => _key.TrimToOption().IfNone(Name);

        public virtual string DisplayName => _displayName.TrimToOption().Map(Tr).IfNone(Key);

        [Node(false)] private Option<ICondition<ISlotItem>> _allowedFor = None;

        [Export] private string _key;

        [Export] private string _displayName;

        public virtual bool AllowedFor(ISlotItem context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return !_allowedFor.Exists(c => !c.Matches(context));
        }

        public bool AllowedFor(object context) => context is ISlotItem item && AllowedFor(item);
    }
}
