using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Attribute;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public class SpeedAttribute : Attribute.Attribute
    {
        public SpeedAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Option<IAttribute> min,
            Option<IAttribute> max,
            Option<IAttribute> modifier,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            icon,
            min,
            max,
            modifier,
            active,
            loggerFactory)
        {
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return Optional(holder)
                .OfType<ILocomotive>()
                .Map(h => h.Locomotion.OnVelocityChange)
                .ToObservable()
                .Switch()
                .Select(v => v.Length());
        }
    }
}
