using System;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public class NodeEventTracker : EventTracker<Node>
    {
        [NotNull]
        public IObservable<float> OnProcess => _onProcess ?? (_onProcess = new Subject<float>());

        [NotNull]
        public IObservable<float> OnPhysicsProcess => _onPhysicsProcess ?? (_onPhysicsProcess = new Subject<float>());

        [NotNull]
        public IObservable<InputEvent> OnInput => _onInput ?? (_onInput = new Subject<InputEvent>());

        [NotNull]
        public IObservable<InputEvent> OnUnhandledInput =>
            _onUnhandledInput ?? (_onUnhandledInput = new Subject<InputEvent>());

        [NotNull]
        public IObservable<Unit> OnDispose => _onDispose ?? (_onDispose = new Subject<Unit>());

        private Subject<float> _onProcess;

        private Subject<float> _onPhysicsProcess;

        private Subject<InputEvent> _onInput;

        private Subject<InputEvent> _onUnhandledInput;

        private Subject<Unit> _onDispose;

        public override void _Ready()
        {
            base._Ready();

            SetProcess(_onProcess != null);
            SetPhysicsProcess(_onPhysicsProcess != null);
            SetProcessInput(_onInput != null);
            SetProcessUnhandledInput(_onUnhandledInput != null);
            SetProcessUnhandledKeyInput(_onUnhandledInput != null);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            _onProcess?.OnNext(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _onPhysicsProcess?.OnNext(delta);
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            _onInput?.OnNext(@event);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if (@event is InputEventKey) return;

            _onUnhandledInput?.OnNext(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event)
        {
            base._UnhandledKeyInput(@event);

            _onUnhandledInput?.OnNext(@event);
        }

        protected override void Connect(Node parent)
        {
        }

        protected override void Disconnect(Node parent)
        {
            _onDispose?.OnNext(Unit.Default);
            _onDispose?.Dispose();
            _onDispose = null;

            _onProcess?.Dispose();
            _onProcess = null;

            _onPhysicsProcess?.Dispose();
            _onPhysicsProcess = null;

            _onInput?.Dispose();
            _onInput = null;

            _onUnhandledInput?.Dispose();
            _onUnhandledInput = null;
        }
    }
}
