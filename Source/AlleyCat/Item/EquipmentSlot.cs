using System;
using AlleyCat.Condition.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class EquipmentSlot : Slot
    {
        public EquipType EquipType { get; }

        public EquipmentSlot(
            string key,
            string displayName,
            EquipType equipType,
            Option<ICondition<ISlotItem>> allowedCondition,
            ILogger logger) : base(key, displayName, allowedCondition, logger)
        {
            EquipType = equipType;
        }

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
