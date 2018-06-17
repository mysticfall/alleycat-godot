using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Item;
using AlleyCat.Item.Generic;

namespace AlleyCat.Character
{
    [Singleton(typeof(IEquipmentContainer), typeof(ISlotContainer<EquipmentSlot, Equipment>))]
    public class CharacterEquipments : EquipmentContainer
    {
        protected override IEquipmentHolder Holder => _character;

        public override IReadOnlyDictionary<string, EquipmentSlot> Slots =>
            _slots ?? Enumerable.Empty<EquipmentSlot>().ToDictionary(s => s.Key);

        [Ancestor] private ICharacter _character;

        private IReadOnlyDictionary<string, EquipmentSlot> _slots;

        [PostConstruct(true)]
        protected virtual void OnInitialize()
        {
            var slots = _character.Race?.EquipmentSlots.ToDictionary(s => s.Key);

            if (slots != null)
            {
                _slots = slots;
            }
        }
    }
}
