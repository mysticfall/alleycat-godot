using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Control.Generic;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public abstract class Input<T> : Node, IInput<T>
    {
        public virtual string Key => Name;

        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private IObservable<T> _observable;

        public override void _Ready()
        {
            base._Ready();

            _observable = CreateObservable();

            Debug.Assert(_observable != null, "CreateObservable() != null");
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            _observable = null;
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            Ensure.Any.IsNotNull(observer, nameof(observer));

            return _observable.Where(v => Active).Subscribe(observer);
        }

        [NotNull]
        protected abstract IObservable<T> CreateObservable();

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();

            base.Dispose(disposing);
        }
    }
}
