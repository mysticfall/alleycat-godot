using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    public class AccumulateAttribute : Attribute
    {
        public IEnumerable<IAttribute> Sources { get; }

        private readonly BehaviorSubject<IObservable<float>> _value;

        public AccumulateAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            float initialValue,
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

            _value = CreateSubject(Return(initialValue));
        }

        public override void Initialize(IAttributeHolder holder)
        {
            base.Initialize(holder);

            Sources.Map(a => a.OnChange).Iter(Add);
        }

        public void Add(float value) => Add(Return(value));

        public void Add(IObservable<float> value)
        {
            Ensure.That(value, nameof(value)).IsNotNull();

            _value.OnNext(value);
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            var source = _value
                .SelectMany(identity)
                .WithLatestFrom(OnModifierChange, (v, m) => v * m)
                .WithLatestFrom(OnRangeChange, (value, range) => (value, range));

            return source
                .Scan(0f, (sum, v) => v.range.Clamp(sum + v.value))
                .CombineLatest(OnRangeChange, (v, r) => r.Clamp(v));
        }
    }
}
