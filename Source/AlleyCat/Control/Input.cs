using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Control.Generic;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public abstract class Input<T> : GameObject, IInput<T>
    {
        public virtual string Key { get; }

        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public IInputSource Source { get; }

        private readonly BehaviorSubject<bool> _active;

        protected Input(
            string key,
            IInputSource source,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(source, nameof(source)).IsNotNull();

            Key = key;
            Source = source;

            _active = CreateSubject(active);
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            var source = CreateObservable().Where(_ => Valid && Active);

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                source = source.Do(v => this.LogTrace("Input value changed: '{}'.", v));
            }

            return source.Subscribe(observer);
        }

        protected abstract IObservable<T> CreateObservable();
    }
}
