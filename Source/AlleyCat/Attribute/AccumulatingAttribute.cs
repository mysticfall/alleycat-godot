using System;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    public class AccumulatingAttribute : Attribute
    {
        public Option<IAttribute> Generator { get; }

        public TimeSpan Period { get; }

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

        private readonly BehaviorSubject<IObservable<float>> _value;

        public AccumulatingAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            float initialValue,
            Option<IAttribute> min,
            Option<IAttribute> max,
            Option<IAttribute> modifier,
            Option<IAttribute> generator,
            TimeSpan period,
            ProcessMode processMode,
            ITimeSource timeSource,
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
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Generator = generator;
            Period = period;
            ProcessMode = processMode;
            TimeSource = timeSource;

            _value = CreateSubject(Return(initialValue));
        }

        public override void Initialize(IAttributeHolder holder)
        {
            base.Initialize(holder);

            Generator.Iter(generator =>
            {
                generator.Initialize(holder);

                var increments = Interval(Period, TimeSource.Scheduler(ProcessMode))
                    .Where(_ => Active)
                    .WithLatestFrom(generator.OnChange, (_, v) => v);

                Add(increments);
            });
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
