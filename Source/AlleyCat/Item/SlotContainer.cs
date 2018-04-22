using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Condition;
using AlleyCat.Item.Generic;
using EnsureThat;
using Godot;

namespace AlleyCat.Item
{
    public abstract class SlotContainer<TSlot, TItem> : Directory<TItem>, ISlotContainer<TSlot, TItem>
        where TSlot : ISlot
        where TItem : ISlotItem
    {
        public abstract IReadOnlyDictionary<string, TSlot> Slots { get; }

        public IObservable<TItem> OnAdd => _onAdd;

        public IObservable<TItem> OnRemove => _onRemove;

        protected override string GetKey(TItem item) => item.Slot;

        private readonly Subject<TItem> _onAdd = new Subject<TItem>();

        private readonly Subject<TItem> _onRemove = new Subject<TItem>();

        public virtual TItem Add(TItem item)
        {
            Ensure.Any.IsNotNull(item, nameof(item));

            var key = item.Slot;

            Ensure.Bool.IsTrue(
                ContainsKey(item.Slot),
                nameof(item),
                opt => opt.WithMessage($"Unknown slot: '{key}'."));

            var slot = Slots[item.Slot];

            Ensure.Bool.IsTrue(
                slot.AllowedFor(item) && item.AllowedFor(this),
                nameof(item),
                opt => opt.WithMessage($"'{item}' is not allowed in this container: '{this}'."));

            var replacing = this[key];

            var node = item as Node;

            if (node != null)
            {
                ItemsParent.AddChild(node);
            }

            Cache[key] = item;

            _onAdd.OnNext(item);

            return replacing;
        }

        public virtual void Remove(TItem item)
        {
            Ensure.Any.IsNotNull(item, nameof(item));

            if (!Cache.Remove(item.Key)) return;

            var node = item as Node;

            if (node != null)
            {
                ItemsParent.RemoveChild(node);
            }

            _onRemove.OnNext(item);
        }

        public TItem Clear(string slot)
        {
            var item = this[slot];

            if (item == null) return default(TItem);

            Remove(item);

            return item;
        }

        protected override void Dispose(bool disposing)
        {
            _onAdd?.Dispose();
            _onRemove?.Dispose();

            base.Dispose(disposing);
        }
    }
}
