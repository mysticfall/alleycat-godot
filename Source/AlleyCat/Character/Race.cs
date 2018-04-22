using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Item;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character
{
    public class Race : AutowiredNode, IRace
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        [Node("Slots/Equipments")]
        public IEnumerable<EquipmentSlot> EquipmentSlots { get; private set; }

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;
    }
}
