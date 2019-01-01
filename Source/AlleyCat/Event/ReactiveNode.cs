using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Event
{
    [NonInjectable]
    public class ReactiveNode : AutowiredNode, IReactiveObject, ITimeSource, IInputSource
    {
        public IObservable<bool> Initialized => _delegate.Initialized;

        public IObservable<bool> Disposed => _delegate.Disposed;

        public IObservable<float> OnProcess => _onProcess.IfNone(() =>
        {
            SetProcess(true);

            return (_onProcess = new Subject<float>()).Head();
        }).AsObservable();

        public IObservable<float> OnPhysicsProcess => _onPhysicsProcess.IfNone(() =>
        {
            SetPhysicsProcess(true);

            return (_onPhysicsProcess = new Subject<float>()).Head();
        }).AsObservable();

        public IScheduler Scheduler => _scheduler.IfNone(
            () => (_scheduler = new ProcessScheduler(OnProcess)).Head());

        public IScheduler PhysicsScheduler => _physicsScheduler.IfNone(
            () => (_physicsScheduler = new ProcessScheduler(OnPhysicsProcess)).Head());

        public IObservable<InputEvent> OnInput => _onInput.IfNone(() =>
        {
            SetProcessInput(true);

            return (_onInput = new Subject<InputEvent>()).Head();
        }).AsObservable();

        public IObservable<InputEvent> OnUnhandledInput => _onUnhandledInput.IfNone(() =>
        {
            SetProcessUnhandledInput(true);

            return (_onUnhandledInput = new Subject<InputEvent>()).Head();
        }).AsObservable();

        private readonly ReactiveObject _delegate = new ReactiveObject();

        private Option<Subject<float>> _onProcess;

        private Option<Subject<float>> _onPhysicsProcess;

        private Option<Subject<InputEvent>> _onInput;

        private Option<Subject<InputEvent>> _onUnhandledInput;

        private Option<IScheduler> _scheduler;

        private Option<IScheduler> _physicsScheduler;

        private bool _valid;

        protected ReactiveNode()
        {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
        }

        public ReactiveNode(string name) : this()
        {
            Ensure.That(name, nameof(name)).IsNotNullOrEmpty();

            Name = name;
        }

        public override void _Ready()
        {
            base._Ready();

            _delegate.Initialize();
        }

        public BehaviorSubject<T> CreateSubject<T>(T initialValue) => _delegate.CreateSubject(initialValue);

        public ISubject<T> CreateSubject<T>() => _delegate.CreateSubject<T>();

        public override void _Process(float delta)
        {
            base._Process(delta);

            _onProcess.Iter(l => l.OnNext(delta));
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _onPhysicsProcess.Iter(l => l.OnNext(delta));
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

        public void SetInputAsHandled() => GetTree().SetInputAsHandled();

        protected override void Dispose(bool disposing)
        {
            PreDestroy();

            _onProcess.Iter(l => l.CompleteAndDispose());
            _onProcess = Prelude.None;

            _onPhysicsProcess.Iter(l => l.CompleteAndDispose());
            _onPhysicsProcess = Prelude.None;

            _onInput.Iter(l => l.CompleteAndDispose());
            _onInput = Prelude.None;

            _onUnhandledInput.Iter(l => l.CompleteAndDispose());
            _onUnhandledInput = Prelude.None;

            _scheduler.OfType<IDisposable>().Iter(d => d.DisposeQuietly());
            _scheduler = Prelude.None;

            _physicsScheduler.OfType<IDisposable>().Iter(d => d.DisposeQuietly());
            _physicsScheduler = Prelude.None;

            _delegate.Dispose();

            base.Dispose(disposing);
        }

        protected virtual void PreDestroy()
        {
        }
    }
}
