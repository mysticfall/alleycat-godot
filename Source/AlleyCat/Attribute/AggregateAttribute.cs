using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class AggregateAttribute : Attribute
    {
        public IEnumerable<IAttribute> Sources { get; }

        public AggregateAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            IEnumerable<IAttribute> sources,
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
            Ensure.That(sources, nameof(sources)).IsNotNull();

            Sources = sources;

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("Using source attributes: {}", string.Join(", ", Sources));
            }
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return Sources
                .Select(a => a.OnChange.DistinctUntilChanged())
                .CombineLatest(values => values.Sum())
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
