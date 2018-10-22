using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Condition.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

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
            IObservable<TItem> OnAdd { get; }

            IObservable<TItem> OnRemove { get; }

            IObservable<IEnumerable<TItem>> OnItemsChange { get; }

            Map<string, TSlot> Slots { get; }

            void Add(TItem item);

            void Remove(TItem item);

            Option<TItem> Clear(string slot);
        }

        public static class SlotContainerExtensions
        {
            public static Option<string> FindSlot<TSlot, TItem>(
                this ISlotContainer<TSlot, TItem> container,
                TItem item)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.That(container, nameof(container)).IsNotNull();
                Ensure.That(item, nameof(item)).IsNotNull();

                return container.Filter(t => t.Value == item).Map(t => t.Key).HeadOrNone();
            }

            public static Option<TItem> FindItem<TSlot, TItem>(
                this ISlotContainer<TSlot, TItem> container,
                string slot)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.That(container, nameof(container)).IsNotNull();
                Ensure.That(slot, nameof(slot)).IsNotNull();

                return container.Values.Find(i => i.Slot == slot || i.AdditionalSlots.Contains(slot));
            }

            public static Set<string> OccupiedSlots<TSlot, TItem>(
                this ISlotContainer<TSlot, TItem> container)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.Any.IsNotNull(container, nameof(container));

                return toSet(container.Values.Bind(t => t.GetAllSlots()).Distinct());
            }

            public static bool IsSlotAvailable<TSlot, TItem>(
                this ISlotContainer<TSlot, TItem> container,
                string slot)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.Any.IsNotNull(container, nameof(container));

                return OccupiedSlots(container).Exists(s => s == slot);
            }

            public static IEnumerable<TItem> Replace<TSlot, TItem>(
                this ISlotContainer<TSlot, TItem> container,
                TItem item)
                where TSlot : ISlot
                where TItem : Node, ISlotItem
            {
                Ensure.Any.IsNotNull(container, nameof(container));
                Ensure.Any.IsNotNull(item, nameof(item));

                var allSlots = item.GetAllSlots();
                var occupiedSlots = OccupiedSlots(container);
                var slotsToFree = allSlots.Intersect(occupiedSlots);

                var itemsToFree = toSet(
                    slotsToFree.Bind(s => FindItem(container, s)).Distinct());

                itemsToFree.Iter(container.Remove);

                container.Add(item);

                return itemsToFree;
            }
        }
    }
}
