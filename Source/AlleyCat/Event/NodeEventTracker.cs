using System;
using System.Reactive.Subjects;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Event
{
    public class NodeEventTracker : EventTracker<Node>
    {
        public IObservable<float> OnProcess => _onProcess.Head();

        public IObservable<float> OnPhysicsProcess => _onPhysicsProcess.Head();

        public IObservable<InputEvent> OnInput => _onInput.Head();

        public IObservable<InputEvent> OnUnhandledInput => _onUnhandledInput.Head();

        private Option<Subject<float>> _onProcess = Some(_ => new Subject<float>());

        private Option<Subject<float>> _onPhysicsProcess = Some(_ => new Subject<float>());

        private Option<Subject<InputEvent>> _onInput = Some(_ => new Subject<InputEvent>());

        private Option<Subject<InputEvent>> _onUnhandledInput = Some(_ => new Subject<InputEvent>());

        public override void _Ready()
        {
            base._Ready();

            SetProcess(_onProcess.IsSome);
            SetPhysicsProcess(_onPhysicsProcess.IsSome);
            SetProcessInput(_onInput.IsSome);
            SetProcessUnhandledInput(_onUnhandledInput.IsSome);
            SetProcessUnhandledKeyInput(_onUnhandledInput.IsSome);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            _onProcess.Iter(p => p.OnNext(delta));
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _onPhysicsProcess.Iter(p => p.OnNext(delta));
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            _onInput.Iter(i => i.OnNext(@event));
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if (@event is InputEventKey) return;

            _onUnhandledInput.Iter(i => i.OnNext(@event));
        }

        public override void _UnhandledKeyInput(InputEventKey @event)
        {
            base._UnhandledKeyInput(@event);

            _onUnhandledInput.Iter(i => i.OnNext(@event));
        }

        protected override void Disconnect(Node parent)
        {
            _onProcess.Iter(p => p.DisposeQuietly());
            _onPhysicsProcess.Iter(p => p.DisposeQuietly());

            _onInput.Iter(i => i.DisposeQuietly());
            _onUnhandledInput.Iter(i => i.DisposeQuietly());
        }
    }
}
