using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class AggregatingAttribute : Attribute
    {
        public IEnumerable<IAttribute> Attributes { get; }

        public AggregatingAttribute(
            string key,
            string displayName,
            Option<string> description,
            IEnumerable<IAttribute> attributes,
            Option<IAttribute> min,
            Option<IAttribute> max,
            Option<IAttribute> modifier,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            min,
            max,
            modifier,
            active,
            loggerFactory)
        {
            Ensure.That(attributes, nameof(attributes)).IsNotNull();

            Attributes = attributes;
        }

        public override void Initialize(IAttributeSet attributes)
        {
            base.Initialize(attributes);

            Attributes.Iter(m => m.Initialize(attributes));
        }

        protected override IObservable<float> CreateObservable(IAttributeSet attributes)
        {
            Ensure.That(attributes, nameof(attributes)).IsNotNull();

            return Attributes
                .Select(a => a.OnChange)
                .CombineLatest(values => values.Sum())
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
