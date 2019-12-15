using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Game;
using AlleyCat.Item.Generic;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class SlotContainer<TSlot, TItem> : GameNode, ISlotContainer<TSlot, TItem>
        where TSlot : ISlot
        where TItem : class, ISlotItem
    {
        public abstract Map<string, TSlot> Slots { get; }

        public Map<string, TItem> Items => _items.Value;

        public IObservable<TItem> OnAdd => _onAdd.AsObservable();

        public IObservable<TItem> OnRemove => _onRemove.AsObservable();

        public IObservable<IEnumerable<TItem>> OnItemsChange => _items.Select(v => v.Values);

        protected abstract IEnumerable<TItem> InitialItems { get; }

        private readonly ISubject<TItem> _onAdd;

        private readonly ISubject<TItem> _onRemove;

        private readonly BehaviorSubject<Map<string, TItem>> _items;

        protected SlotContainer(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _onAdd = CreateSubject<TItem>();
            _onRemove = CreateSubject<TItem>();
            _items = CreateSubject(Map<string, TItem>());
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            _items.OnNext(toMap(InitialItems.Filter(v => Slots.Keys.Contains(v.Slot)).Map(i => (i.Slot, i))));
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

            _items.OnNext(Items.Add(item.Slot, item));

            this.LogDebug("Item '{}' is added to the container.", item);

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
                    $"Item '{item.Key}' is not added in this container's slot: '{item.Slot}'.");
            }

            DoRemove(item);

            _items.OnNext(Items.Remove(item.Slot));

            this.LogDebug("Item '{}' is removed from the container.", item);

            _onRemove.OnNext(item);
        }

        protected abstract void DoRemove(TItem item);

        public Option<TItem> Clear(string slot)
        {
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
