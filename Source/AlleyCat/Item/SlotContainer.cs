using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Item.Generic;
using EnsureThat;
using Godot;

namespace AlleyCat.Item
{
    public abstract class SlotContainer<TSlot, TItem> : Directory<TItem>, ISlotContainer<TSlot, TItem>
        where TSlot : ISlot
        where TItem : Node, ISlotItem
    {
        public abstract IReadOnlyDictionary<string, TSlot> Slots { get; }

        public IObservable<TItem> OnAdd => _onAdd;

        public IObservable<TItem> OnRemove => _onRemove;

        private readonly Subject<TItem> _onAdd = new Subject<TItem>();

        private readonly Subject<TItem> _onRemove = new Subject<TItem>();

        public virtual void Add(TItem item)
        {
            Ensure.Any.IsNotNull(item, nameof(item));

            Ensure.Bool.IsTrue(
                AllowedFor(item) &&
                item.AllowedFor(this) &&
                Slots.TryGetValue(item.Slot, out var slot) &&
                slot.AllowedFor(item),
                nameof(item),
                opt => opt.WithMessage($"'{item}' is not allowed in this container: '{this}'."));

            DoAdd(item);

            Cache[item.Slot] = item;

            _onAdd.OnNext(item);
        }

        protected abstract void DoAdd(TItem item);

        public virtual void Remove(TItem item)
        {
            Ensure.Any.IsNotNull(item, nameof(item));

            var key = this.Where(t => t.Value == item).Select(t => t.Key).FirstOrDefault();

            if (key == null) return;

            DoRemove(item);

            Cache.Remove(item.Slot);

            _onRemove.OnNext(item);
        }

        protected abstract void DoRemove(TItem item);

        public TItem Clear(string slot)
        {
            Ensure.Any.IsNotNull(slot, nameof(slot));

            if (!ContainsKey(slot)) return default;

            var item = this[slot];

            Remove(item);

            return item;
        }

        public virtual bool AllowedFor(ISlotConfiguration context)
        {
            if (context == null) return false;

            var allSlots = new HashSet<string>(context.GetAllSlots());

            if (!allSlots.All(Slots.ContainsKey)) return false;

            return !new HashSet<string>(this.OccupiedSlots()).Intersect(allSlots).Any();
        }

        public bool AllowedFor(object context) => AllowedFor(context as ISlotConfiguration);

        protected override string GetKey(TItem item) => item.Slot;

        protected override void Dispose(bool disposing)
        {
            _onAdd?.Dispose();
            _onRemove?.Dispose();

            base.Dispose(disposing);
        }
    }
}
