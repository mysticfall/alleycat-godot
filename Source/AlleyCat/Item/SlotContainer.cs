using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Item.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class SlotContainer<TSlot, TItem> : Directory<TItem>, ISlotContainer<TSlot, TItem>
        where TSlot : ISlot
        where TItem : Node, ISlotItem
    {
        public abstract Map<string, TSlot> Slots { get; }

        public IObservable<TItem> OnAdd => _onAdd;

        public IObservable<TItem> OnRemove => _onRemove;

        public IObservable<IEnumerable<TItem>> OnItemsChange =>
            OnAdd.Merge(OnRemove).Select(_ => Values).StartWith(Values);

        private readonly ISubject<TItem> _onAdd;

        private readonly ISubject<TItem> _onRemove;

        protected SlotContainer()
        {
            _onAdd = new Subject<TItem>().AddTo(this);
            _onRemove = new Subject<TItem>().AddTo(this);
        }

        public virtual void Add(TItem item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();
            Ensure.That(
                    AllowedFor(item) &&
                    item.AllowedFor(this) &&
                    Slots.Find(item.Slot).Exists(s => s.AllowedFor(item)),
                    nameof(item),
                    opt => opt.WithMessage($"'{item}' is not allowed in this container: '{this}'."))
                .IsTrue();

            DoAdd(item);

            ClearCache();

            _onAdd.OnNext(item);
        }

        protected abstract void DoAdd(TItem item);

        public virtual void Remove(TItem item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            if (this.All(t => t.Value != item))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(item),
                    $"The item is not added to this container: '{item.Name}'.");
            }

            DoRemove(item);

            ClearCache();

            _onRemove.OnNext(item);
        }

        protected abstract void DoRemove(TItem item);

        public Option<TItem> Clear(string slot)
        {
            Ensure.That(slot, nameof(slot)).IsNotNull();

            var item = this.TryGetValue(slot);

            item.Iter(Remove);

            return item;
        }

        public virtual bool AllowedFor(ISlotConfiguration context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            var allSlots = context.GetAllSlots();

            return allSlots.All(Slots.ContainsKey) && allSlots.Except(this.OccupiedSlots()).Any();
        }

        public bool AllowedFor(object context) => 
            Optional(context).OfType<ISlotConfiguration>().Exists(AllowedFor);

        protected override string GetKey(TItem item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            return item.Slot;
        }
    }
}
