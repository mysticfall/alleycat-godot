using System;
using System.Reactive.Subjects;
using EnsureThat;
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
            string target,
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
            Ensure.That(target, nameof(target)).IsNotNull();

            _target = target;
            _value = CreateSubject(Empty<float>());
        }

        public override void Initialize(IAttributeSet attributes)
        {
            base.Initialize(attributes);

            Target = attributes.TryGetValue(_target);

            _value.OnNext(Target.Select(a => a.OnChange).ToObservable().Switch());
        }

        protected override IObservable<float> CreateObservable(IAttributeSet attributes)
        {
            Ensure.That(attributes, nameof(attributes)).IsNotNull();

            return _value.Switch()
                .CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
