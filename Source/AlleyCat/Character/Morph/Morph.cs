using System;
using System.Reactive.Linq;
using AlleyCat.Character.Morph.Generic;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;

namespace AlleyCat.Character.Morph
{
    public abstract class Morph<TVal, TDef> : BaseNode, IMorph<TVal, TDef>
        where TDef : MorphDefinition<TVal>
    {
        public string Key => Definition.Key;

        public string DisplayName => Definition.DisplayName;

        public TDef Definition { get; }

        IMorphDefinition IMorph.Definition => Definition;

        public TVal Value
        {
            get => _value.Value;
            set => _value.Value = value;
        }

        object IMorph.Value
        {
            get => _value.Value;
            set => _value.Value = (TVal) value;
        }

        public IObservable<TVal> OnChange => _value;

        IObservable<object> IMorph.OnChange => _value.Select(v => (object) v);

        private readonly ReactiveProperty<TVal> _value;

        protected Morph(TDef definition)
        {
            Ensure.That(definition, nameof(definition)).IsNotNull();

            _value = new ReactiveProperty<TVal>(definition.Default).AddTo(this);

            Definition = definition;
            OnChange.Skip(1).Subscribe(Apply).AddTo(this);
        }

        public void Apply() => Apply(Value);

        protected abstract void Apply(TVal value);

        public void Reset() => Value = Definition.Default;
    }
}
