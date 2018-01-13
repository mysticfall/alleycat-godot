using System;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public class EventTracker : Node
    {
        public const string DefaultName = "EventTracker";

        [CanBeNull]
        public IObservable<Unit> OnReady =>_onReady;

        [CanBeNull]
        public IObservable<float> OnProcess =>_onProcess;

        [CanBeNull]
        public IObservable<float> OnPhysicsProcess =>_onPhysicsProcess;

        [CanBeNull]
        public IObservable<InputEvent> OnInput =>_onInput;

        [CanBeNull]
        public IObservable<Unit> OnDispose =>_onDispose;

        private Subject<Unit> _onReady;

        private Subject<float> _onProcess;

        private Subject<float> _onPhysicsProcess;

        private Subject<InputEvent> _onInput;

        private Subject<Unit> _onDispose;

        public override void _Ready()
        {
            base._Ready();

            _onReady =  new Subject<Unit>();
            _onProcess =  new Subject<float>();
            _onPhysicsProcess =  new Subject<float>();
            _onInput =  new Subject<InputEvent>();
            _onDispose =  new Subject<Unit>();

            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);

            _onReady?.OnNext(Unit.Default);
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

        public override void Dispose(bool disposing)
        {
            _onDispose?.OnNext(Unit.Default);
            _onDispose?.Dispose();
            _onDispose = null;

            _onReady?.Dispose();
            _onReady = null;

            _onProcess?.Dispose();
            _onProcess = null;

            _onPhysicsProcess?.Dispose();
            _onPhysicsProcess = null;

            _onInput?.Dispose();
            _onInput = null;

            base.Dispose(disposing);
        }
    }
}
