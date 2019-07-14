using System;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using static System.Reactive.Linq.Observable;

namespace AlleyCat.Attribute
{
    public class DebounceAttribute : DelegateAttribute
    {
        public Option<IAttribute> Gate { get; private set; }

        public TimeSpan Period { get; }

        private readonly Option<string> _gate;

        public DebounceAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            string target,
            Option<string> gate,
            TimeSpan period,
            bool asRatio,
            Map<string, IAttribute> children,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            icon,
            target,
            asRatio,
            children,
            active,
            loggerFactory)
        {
            Period = period;

            _gate = gate;
        }

        public override void Initialize(IAttributeHolder holder)
        {
            Gate = _gate.Bind(v => this.FindAttribute(v, holder));

            base.Initialize(holder);
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            var source = base.CreateObservable(holder);

            Logger.LogDebug("Throttling {} for {} seconds.", Gate.Map(g => g.Key), Period.TotalSeconds);

            var stream =  Gate.Match(
                gate =>
                {
                    var lastActivity = gate.OnChange
                        .StartWith(0f)
                        .Timestamp()
                        .Select(v => v.Timestamp);

                    return source
                        .Timestamp()
                        .WithLatestFrom(
                            lastActivity, (v, t) => v.Timestamp - t > Period ? Return(v.Value) : Empty<float>())
                        .SelectMany(identity);
                },
                () => source.Throttle(Period)
            );

            return stream.CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
