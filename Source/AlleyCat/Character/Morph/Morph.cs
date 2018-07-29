using System;
using System.Reactive.Linq;
using AlleyCat.Character.Morph.Generic;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using JetBrains.Annotations;

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

        private readonly IDisposable _disposable;

        protected Morph([NotNull] TDef definition)
        {
            Ensure.Any.IsNotNull(definition, nameof(definition));

            Definition = definition;

            _value = new ReactiveProperty<TVal>(Definition.Default);

            _disposable = OnChange.Skip(1).Subscribe(Apply);
        }

        public void Apply() => Apply(Value);

        protected abstract void Apply(TVal value);

        public void Reset() => Value = Definition.Default;

        protected override void OnPreDestroy()
        {
            _value?.Dispose();
            _disposable?.Dispose();

            base.OnPreDestroy();
        }
    }
}
