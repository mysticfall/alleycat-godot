using System;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public class EquipmentSlot : Slot
    {
        [Export]
        public EquipType EquipType { get; set; }

        public virtual Node GetParent([NotNull] IEquipmentHolder holder)
        {
            switch (EquipType)
            {
                case EquipType.Attachment:
                    return holder.Markers[Key];
                case EquipType.Rigged:
                    return holder.Skeleton;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
