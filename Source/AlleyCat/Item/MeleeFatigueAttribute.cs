using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Attribute;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public class MeleeFatigueAttribute : Attribute.Attribute
    {
        public MeleeFatigueAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            icon,
            children,
            active,
            loggerFactory)
        {
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            var item = Optional(holder)
                .OfType<IEquipmentHolder>()
                .Map(h => h.Equipments.OnItemsChange)
                .ToObservable()
                .Switch()
                .Select(i => i.Find(v =>
                    v.ActiveConfiguration.Exists(c => c is MeleeToolConfiguration)).ToObservable())
                .Switch();

            var swing = item
                .Select(i => i.ActiveConfiguration
                    .OfType<MeleeToolConfiguration>()
                    .Select(c => c.OnSwing)
                    .ToObservable()
                    .Switch())
                .Switch();

            var speed = swing
                .Timestamp()
                .Pairwise()
                .Select(v =>
                {
                    var (item1, item2) = v;
                    var elapsed = (item2.Timestamp - item1.Timestamp).TotalSeconds;
                    var diff = Math.Abs(item2.Value - item1.Value);

                    return (float) (diff / elapsed);
                });

            var weight = item.Select(i => i.Node.Weight);

            return speed
                .WithLatestFrom(weight, (s, w) =>  s * w)
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
