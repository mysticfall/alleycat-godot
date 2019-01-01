using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Character.Morph.Generic;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    public abstract class Morph<TVal, TDef> : ReactiveObject, IMorph<TVal, TDef>, ILoggable
        where TDef : MorphDefinition<TVal>
    {
        public string Key => Definition.Key;

        public string DisplayName => Definition.DisplayName;

        public TDef Definition { get; }

        public ILogger Logger { get; }

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

        protected Morph(TDef definition, ILoggerFactory loggerFactory)
        {
            Ensure.That(definition, nameof(definition)).IsNotNull();
            Ensure.That(loggerFactory, nameof(loggerFactory)).IsNotNull();

            _value = CreateSubject(definition.Default);

            Definition = definition;
            Logger = loggerFactory.CreateLogger(this.GetLogCategory());

            OnChange
                .Skip(1)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(Apply, this);
        }

        public void Apply() => Apply(Value);

        protected abstract void Apply(TVal value);

        public void Reset() => Value = Definition.Default;
    }
}
