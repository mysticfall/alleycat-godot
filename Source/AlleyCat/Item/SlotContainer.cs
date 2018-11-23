using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Item.Generic;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class SlotContainer<TSlot, TItem> : GameObject, ISlotContainer<TSlot, TItem>
        where TSlot : ISlot
        where TItem : class, ISlotItem
    {
        public abstract Map<string, TSlot> Slots { get; }

        public Map<string, TItem> Items { get; private set; }

        public IObservable<TItem> OnAdd => _onAdd.AsObservable();

        public IObservable<TItem> OnRemove => _onRemove.AsObservable();

        public IObservable<IEnumerable<TItem>> OnItemsChange =>
            OnAdd.Merge(OnRemove).Select(_ => Items.Values).StartWith(Items.Values);

        protected abstract IEnumerable<TItem> InitialItems { get; }

        private readonly ISubject<TItem> _onAdd;

        private readonly ISubject<TItem> _onRemove;

        protected SlotContainer(ILogger logger) : base(logger)
        {
            _onAdd = new Subject<TItem>().DisposeWith(this);
            _onRemove = new Subject<TItem>().DisposeWith(this);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Items = InitialItems.Filter(v => Slots.Keys.Contains(v.Slot)).ToMap();
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

            Items = Items.Add(item.Slot, item);

            _onAdd.OnNext(item);
        }

        protected abstract void DoAdd(TItem item);

        public virtual void Remove(TItem item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            if (!Items.Keys.Contains(item.Slot))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(item),
                    $"The item is not added to this container: '{item.Key}'.");
            }

            DoRemove(item);

            Items = Items.Remove(item.Slot);

            _onRemove.OnNext(item);
        }

        protected abstract void DoRemove(TItem item);

        public Option<TItem> Clear(string slot)
        {
            Ensure.That(slot, nameof(slot)).IsNotNull();

            var item = Items.Find(slot);

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
    }
}
