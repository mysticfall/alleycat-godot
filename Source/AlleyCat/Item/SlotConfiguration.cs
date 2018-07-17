using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public class SlotConfiguration : AutowiredNode, ISlotConfiguration
    {
        public string Key => _key ?? Name;

        public string Slot => _slot ?? Key;

        public IEnumerable<string> AdditionalSlots => _additionalSlots.TrimToEnumerable();

        public IEnumerable<string> AllSlots => new[] {Slot}.Concat(AdditionalSlots);

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _slot;

        [Export, UsedImplicitly] private string _additionalSlots;
    }
}
