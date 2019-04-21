using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Item.Generic;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public class EquipmentContainer : SlotContainer<EquipmentSlot, Equipment>, IEquipmentContainer
    {
        public IEquipmentHolder Holder { get; }

        public override Map<string, EquipmentSlot> Slots { get; }

        protected override IEnumerable<Equipment> InitialItems =>
            Slots.Values
                .Map(s => s.GetParent(Holder))
                .Distinct()
                .Bind(p => p.GetChildComponents<Equipment>());

        private const string AnimationEventPrefix = "equipments";

        private const string AnimationEventKeyMorph = "morph";

        public EquipmentContainer(
            IEnumerable<EquipmentSlot> slots,
            IEquipmentHolder holder,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(slots, nameof(slots)).IsNotNull();
            Ensure.That(holder, nameof(holder)).IsNotNull();

            Holder = holder;
            Slots = slots.ToMap();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Slots.Values.Iter(s => this.LogDebug("Found slot: '{}'.", s));

                this.LogDebug("Equipping initial items.");
            }

            if (Holder is IAnimatable animatable)
            {
                animatable.AnimationManager.OnAnimationEvent
                    .Where(e => e.Path.HeadOrNone().Contains(AnimationEventPrefix))
                    .Select(e => e.Path
                        .Skip(1)
                        .HeadOrNone()
                        .Bind(this.FindItemInSlot)
                        .Map(v => (@event: e, equipment: v)).ToObservable())
                    .Switch()
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(v => AnimateEquipment(v.equipment, v.@event), this);
            }

            Items.Values.Iter(v => v.Equip(Holder));
        }

        protected override void DoAdd(Equipment item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            this.LogDebug("Equipping item '{}' to '{}'.", item, Holder);

            var transform = item.GetGlobalTransform();

            Optional(item.Node.GetParent()).Iter(p => p.RemoveChild(item.Node));

            var parent = Slots[item.Slot].GetParent(Holder);

            parent.AddChild(item.Node);

            item.SetGlobalTransform(transform);

            item.Node.SetOwner(parent.GetOwner());
            item.Equip(Holder);
        }

        protected override void DoRemove(Equipment item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            this.LogDebug("Removing item '{}' from '{}'.", item, Holder);

            var transform = item.GetGlobalTransform();

            item.Unequip(Holder);

            Optional(item.Node.GetParent()).Iter(p => p.RemoveChild(item.Node));

            item.SetGlobalTransform(transform);
        }

        public override bool AllowedFor(ISlotConfiguration context) =>
            (context is EquipmentConfiguration || context is Equipment) && base.AllowedFor(context);

        protected virtual void AnimateEquipment(Equipment equipment, IAnimationEvent @event)
        {
            Ensure.That(equipment, nameof(equipment)).IsNotNull();
            Ensure.That(@event, nameof(@event)).IsNotNull();

            var path = @event.Path.ToSeq().Skip(2);

            if (!path.Any()) return;

            path.Deconstruct(out var command, out var args);

            switch (@event)
            {
                case ValueChangeEvent v when command == AnimationEventKeyMorph:
                    var key = args.HeadOrNone();
                    var morph = key.Bind(equipment.Morphs.Morphs.Find);

                    morph.Iter(m => m.Value = v.Value);

                    break;
            }
        }

        protected override void PreDestroy()
        {
            Items.Values.Iter(v => v.DisposeQuietly());

            base.PreDestroy();
        }
    }
}
