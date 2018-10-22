using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Item;
using Godot;

namespace AlleyCat.Character
{
    public class Race : AutowiredNode, IRace
    {
        public string Key => _key.TrimToOption().IfNone(Name);

        public virtual string DisplayName => _displayName.TrimToOption().Map(Tr).IfNone(Key);

        [Node("Slots/Equipments")]
        public IEnumerable<EquipmentSlot> EquipmentSlots { get; private set; } = Enumerable.Empty<EquipmentSlot>();

        [Export] private string _key;

        [Export] private string _displayName;
    }
}
