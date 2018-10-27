using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Item;
using AlleyCat.Item.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    [Singleton(typeof(IEquipmentContainer), typeof(ISlotContainer<EquipmentSlot, Equipment>))]
    public class CharacterEquipments : EquipmentContainer
    {
        protected override IEquipmentHolder Holder => _character.Head();

        public override Map<string, EquipmentSlot> Slots => _slots;

        [Ancestor] private Option<ICharacter> _character;

        private Map<string, EquipmentSlot> _slots = Map<string, EquipmentSlot>();

        protected override void OnInitialize()
        {
            _slots = _character.Bind(c => c.Race.EquipmentSlots).ToMap();

            base.OnInitialize();
        }
    }
}
