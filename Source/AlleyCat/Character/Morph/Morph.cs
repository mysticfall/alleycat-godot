using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Character.Morph.Generic;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Character.Morph
{
    public abstract class Morph<TVal, TDef> : IMorph<TVal, TDef>, IDisposableCollector
        where TDef : MorphDefinition<TVal>
    {
        public string Key => Definition.Key;

        public string DisplayName => Definition.DisplayName;

        public TDef Definition { get; }

        IMorphDefinition IMorph.Definition => Definition;

        public virtual TVal Value
        {
            get => _value.Value;
            set => _value.OnNext(value);
        }

        object IMorph.Value
        {
            get => Value;
            set => Value = (TVal) value;
        }

        public IObservable<TVal> OnChange => _value.AsObservable();

        IObservable<object> IMorph.OnChange => _value.Select(v => (object) v);

        private readonly BehaviorSubject<TVal> _value;

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        protected Morph(TDef definition)
        {
            Ensure.That(definition, nameof(definition)).IsNotNull();

            _value = new BehaviorSubject<TVal>(definition.Default).DisposeWith(this);

            Definition = definition;
            OnChange.Skip(1).Subscribe(Apply).DisposeWith(this);
        }

        public void Apply() => Apply(Value);

        protected abstract void Apply(TVal value);

        public void Reset() => Value = Definition.Default;

        public void Collect(IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            _disposables += disposable;
        }

        public virtual void Dispose()
        {
            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();
        }
    }
}
