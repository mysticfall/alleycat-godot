using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class VariableAttribute : Attribute
    {
        private readonly BehaviorSubject<float> _value;

        public VariableAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            float initialValue,
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
            _value = CreateSubject(initialValue);
        }

        public void Set(float value) => _value.OnNext(value);

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return _value.CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
