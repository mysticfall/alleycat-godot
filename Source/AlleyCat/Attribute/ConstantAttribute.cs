using System;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    public class ConstantAttribute : Attribute
    {
        private readonly float _value;

        public ConstantAttribute(
            string key,
            string displayName,
            Option<string> description,
            float value,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            None,
            None,
            None,
            active,
            loggerFactory)
        {
            _value = value;
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return Return(_value).CombineLatest(OnModifierChange, OnRangeChange, (v, m, r) => r.Clamp(v * m));
        }
    }
}
