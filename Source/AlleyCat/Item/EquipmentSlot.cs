using System;
using EnsureThat;
using Godot;

namespace AlleyCat.Item
{
    public class EquipmentSlot : Slot
    {
        [Export]
        public EquipType EquipType { get; set; }

        public virtual Node GetParent(IEquipmentHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            switch (EquipType)
            {
                case EquipType.Attachment:
                    return holder.Markers.Find(Key).Match(v => v, () =>
                        throw new ArgumentOutOfRangeException(nameof(holder),
                            $"The specified equipment holder does not contain slot '{Key}'."));
                case EquipType.Rigged:
                    return holder.Skeleton;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
