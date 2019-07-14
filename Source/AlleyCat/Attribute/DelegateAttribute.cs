using System;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;

namespace AlleyCat.Attribute
{
    public class DelegateAttribute : Attribute
    {
        public Option<IAttribute> Target { get; private set; }

        public bool AsRatio { get; }

        private readonly string _target;

        public DelegateAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            string target,
            bool asRatio,
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
            Ensure.That(target, nameof(target)).IsNotNull();

            AsRatio = asRatio;

            _target = target;
        }

        public override void Initialize(IAttributeHolder holder)
        {
            Target = this.FindAttribute(_target, holder);

            Logger.LogDebug("Delegating to {}.", Target);

            base.Initialize(holder);
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            var target = AsRatio ? Target.Map(a => a.OnRatioChange()) : Target.Map(a => a.OnChange);

            return target.ToObservable().Switch()
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
