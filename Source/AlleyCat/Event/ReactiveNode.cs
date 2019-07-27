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

        public IObservable<float> OnProcess
        {
            get
            {
                if (_onProcess == null)
                {
                    SetProcess(true);

                    _onProcess = new Subject<float>();
                }

                return _onProcess.AsObservable();
            }
        }

        public IObservable<float> OnPhysicsProcess
        {
            get
            {
                if (_onPhysicsProcess == null)
                {
                    SetPhysicsProcess(true);

                    _onPhysicsProcess = new Subject<float>();
                }

                return _onPhysicsProcess.AsObservable();
            }
        }

        public IScheduler Scheduler => _scheduler.IfNone(
            () => (_scheduler = new ProcessScheduler(OnProcess)).Head());

        public IScheduler PhysicsScheduler => _physicsScheduler.IfNone(
            () => (_physicsScheduler = new ProcessScheduler(OnPhysicsProcess)).Head());

        public IObservable<InputEvent> OnInput
        {
            get
            {
                if (_onInput == null)
                {
                    SetProcessInput(true);

                    _onInput = new Subject<InputEvent>();
                }

                return _onInput.AsObservable();
            }
        }

        public IObservable<InputEvent> OnUnhandledInput
        {
            get
            {
                if (_onUnhandledInput == null)
                {
                    SetProcessUnhandledInput(true);

                    _onUnhandledInput = new Subject<InputEvent>();
                }

                return _onUnhandledInput.AsObservable();
            }
        }

        private readonly ReactiveObject _delegate = new ReactiveObject();

        private Subject<float> _onProcess;

        private Subject<float> _onPhysicsProcess;

        private Subject<InputEvent> _onInput;

        private Subject<InputEvent> _onUnhandledInput;

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

        public void SetInputAsHandled() => GetTree().SetInputAsHandled();

        protected override void Dispose(bool disposing)
        {
            PreDestroy();

            _onProcess?.CompleteAndDispose();
            _onProcess = null;

            _onPhysicsProcess?.CompleteAndDispose();
            _onPhysicsProcess = null;

            _onInput?.CompleteAndDispose();
            _onInput = null;

            _onUnhandledInput?.CompleteAndDispose();
            _onUnhandledInput = null;

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
