using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Control.Generic;
using AlleyCat.Event;
using EnsureThat;

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

        protected Input(string key, IInputSource source, bool active = true)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(source, nameof(source)).IsNotNull();

            Key = key;
            Source = source;

            _active = new BehaviorSubject<bool>(active).AddTo(this);
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            Ensure.Any.IsNotNull(observer, nameof(observer));

            return CreateObservable().Where(_ => Valid && Active).Subscribe(observer);
        }

        protected abstract IObservable<T> CreateObservable();
    }
}
