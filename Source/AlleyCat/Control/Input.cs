using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Control.Generic;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public abstract class Input<T> : BaseNode, IInput<T>
    {
        public virtual string Key => Name;

        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        private readonly ReactiveProperty<bool> _active;

        private Option<IObservable<T>> _observable;

        protected Input()
        {
            _active = new ReactiveProperty<bool>(true).AddTo(this);
        }

        public override void _Ready()
        {
            base._Ready();

            _observable = Some(CreateObservable());

            Debug.Assert(_observable.IsSome, "CreateObservable() != null");
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            Ensure.Any.IsNotNull(observer, nameof(observer));

            return _observable.Match(o => o.Where(_ => Valid && Active).Subscribe(observer)
                , () => throw new InvalidOperationException("The input has not been initialized yet."));
        }

        protected abstract IObservable<T> CreateObservable();
    }
}
