using System;
using System.Reactive.Subjects;
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

        private readonly string _target;

        private readonly BehaviorSubject<IObservable<float>> _value;

        public DelegateAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            string target,
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
            Ensure.That(target, nameof(target)).IsNotNull();

            _target = target;
            _value = CreateSubject(Empty<float>());
        }

        public override void Initialize(IAttributeHolder holder)
        {
            base.Initialize(holder);

            Target = holder.Attributes.TryGetValue(_target);

            _value.OnNext(Target.Select(a => a.OnChange).ToObservable().Switch());
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return _value.Switch()
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
