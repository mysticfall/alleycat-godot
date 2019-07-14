using System;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;

namespace AlleyCat.Attribute
{
    public class SampleAttribute : DelegateAttribute
    {
        public TimeSpan Period { get; }

        public ProcessMode ProcessMode { get; }

        protected ITimeSource TimeSource { get; }

        public SampleAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            string target,
            TimeSpan period,
            ProcessMode processMode,
            ITimeSource timeSource,
            Map<string, IAttribute> children,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            icon,
            target,
            false,
            children,
            active,
            loggerFactory)
        {
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            Period = period;
            ProcessMode = processMode;
            TimeSource = timeSource;
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            return Interval(Period, TimeSource.Scheduler(ProcessMode))
                .Where(_ => Active)
                .WithLatestFrom(base.CreateObservable(holder), (_, v) => v)
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));;
        }
    }
}
