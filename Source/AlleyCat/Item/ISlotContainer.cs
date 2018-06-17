using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Condition.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public interface ISlotContainer : IEnumerable
    {
    }

    namespace Generic
    {
        public interface ISlotContainer<TSlot, TItem> : ISlotContainer, IReadOnlyDictionary<string, TItem>,
            IRestricted<ISlotConfiguration>
            where TSlot : ISlot
            where TItem : Node, ISlotItem
        {
            [NotNull]
            IObservable<TItem> OnAdd { get; }

            [NotNull]
            IObservable<TItem> OnRemove { get; }

            [NotNull]
            IReadOnlyDictionary<string, TSlot> Slots { get; }

            void Add([NotNull] TItem item);

            void Remove([NotNull] TItem item);

            [CanBeNull]
            TItem Clear([NotNull] string slot);
        }

        public static class SlotContainerExtensions
        {
            [CanBeNull]
            public static string FindSlot<TSlot, TItem>(
                [NotNull] this ISlotContainer<TSlot, TItem> container,
                [NotNull] TItem item)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.Any.IsNotNull(container, nameof(container));
                Ensure.Any.IsNotNull(item, nameof(item));

                return container.Where(t => t.Value == item).Select(t => t.Key).FirstOrDefault();
            }

            [CanBeNull]
            public static TItem FindItem<TSlot, TItem>(
                [NotNull] this ISlotContainer<TSlot, TItem> container,
                [NotNull] string slot)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.Any.IsNotNull(container, nameof(container));
                Ensure.Any.IsNotNull(slot, nameof(slot));

                return container.Values.FirstOrDefault(i => i.Slot == slot || i.AdditionalSlots.Contains(slot));
            }

            [NotNull]
            public static IEnumerable<string> OccupiedSlots<TSlot, TItem>(
                [NotNull] this ISlotContainer<TSlot, TItem> container)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.Any.IsNotNull(container, nameof(container));

                return container.Values.SelectMany(t => t.GetAllSlots()).Distinct();
            }

            public static bool IsSlotAvailable<TSlot, TItem>(
                [NotNull] this ISlotContainer<TSlot, TItem> container,
                [NotNull] string slot)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                return OccupiedSlots(container).FirstOrDefault(s => s == slot) == null;
            }

            [NotNull]
            public static IEnumerable<TItem> Replace<TSlot, TItem>(
                [NotNull] this ISlotContainer<TSlot, TItem> container,
                [NotNull] TItem item)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.Any.IsNotNull(item, nameof(item));

                var allSlots = new HashSet<string>(item.GetAllSlots());
                var occupiedSlots = new HashSet<string>(OccupiedSlots(container));
                var slotsToFree = allSlots.Intersect(occupiedSlots);

                var itemsToFree = slotsToFree
                    .Select(s => FindItem(container, s))
                    .Where(i => i != null)
                    .Distinct()
                    .ToList();

                itemsToFree.ForEach(container.Remove);

                container.Add(item);

                return itemsToFree;
            }
        }
    }
}
