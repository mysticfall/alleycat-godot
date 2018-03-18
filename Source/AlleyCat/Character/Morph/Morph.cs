using System;
using System.Reactive.Linq;
using AlleyCat.Character.Morph.Generic;
using AlleyCat.Event;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public abstract class Morph<TVal, TDef> : IMorph<TVal, TDef> where TDef : MorphDefinition<TVal>
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

        public IObservable<TVal> OnChange => _value;

        IObservable<object> IMorph.OnChange => _value.Select(v => (object) v);

        private readonly ReactiveProperty<TVal> _value;

        protected Morph([NotNull] TDef definition)
        {
            Ensure.Any.IsNotNull(definition, nameof(definition));

            Definition = definition;

            _value = new ReactiveProperty<TVal>(Definition.Default);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _value.Dispose();
        }
    }
}
