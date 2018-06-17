using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    [Singleton(typeof(EquipConfiguration), typeof(ISlotConfiguration), typeof(SlotConfiguration))]
    public class EquipConfiguration : SlotConfiguration, ITaggable
    {
        public IEnumerable<string> Tags => GetGroups().OfType<string>();

        public bool HasTag(string tag) => IsInGroup(tag);

        public void AddTag(string tag) => AddToGroup(tag);

        public void RemoveTag(string tag) => RemoveFromGroup(tag);

        [Export, UsedImplicitly] private PackedScene _equipment;

        public Equipment CreateEquipment() => (Equipment) _equipment?.Instance();
    }
}
