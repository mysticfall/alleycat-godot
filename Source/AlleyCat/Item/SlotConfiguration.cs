using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public class SlotConfiguration : AutowiredNode, ISlotConfiguration
    {
        public string Key => _key.TrimToOption().IfNone(Name);

        public string Slot => _slot.TrimToOption().IfNone(Key);

        public Set<string> AdditionalSlots => toSet(_additionalSlots);

        [Export] private string _key;

        [Export] private string _slot;

        [Export, UsedImplicitly] private Array<string> _additionalSlots;
    }
}
