using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Attribute;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Motion
{
    public class SpeedAttribute : Attribute.Attribute
    {
        public float Threshold { get; }

        public SpeedAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            float threshold,
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
            Threshold = Mathf.Max(threshold, 0);
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return Optional(holder)
                .OfType<ILocomotive>()
                .Map(h => h.Locomotion.OnVelocityChange)
                .ToObservable()
                .Switch()
                .Select(v => v.Length())
                .Select(v => v > Threshold ? v : 0f)
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
