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
        public IEnumerable<IAttribute> Attributes { get; }

        protected override IEnumerable<IAttribute> Children => base.Children.Append(Attributes);

        public AggregateAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            IEnumerable<IAttribute> attributes,
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
            Ensure.That(attributes, nameof(attributes)).IsNotNull();

            Attributes = attributes;
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return Attributes
                .Select(a => a.OnChange.DistinctUntilChanged())
                .CombineLatest(values => values.Sum())
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
