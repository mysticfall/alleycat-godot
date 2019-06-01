using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Item.Generic;
using AlleyCat.Morph;
using Godot;
using LanguageExt;

namespace AlleyCat.Item
{
    public interface IEquipment : ISlotItem<RigidBody>, IEntity, IMarkable, IMorphable
    {
        EquipmentType EquipmentType { get; }

        EquipmentConfiguration Configuration { get; }

        Option<EquipmentConfiguration> ActiveConfiguration { get; }

        Map<string, EquipmentConfiguration> Configurations { get; }

        void Equip(IEquipmentHolder holder);

        void Unequip(IEquipmentHolder holder);
    }
}
