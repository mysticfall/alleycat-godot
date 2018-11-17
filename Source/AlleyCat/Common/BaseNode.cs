using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public class BaseNode : Node, IDisposableCollector, ITimeSource, IInputSource, IValidatable
    {
        public virtual bool Valid => _valid;

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

        private Option<Subject<float>> _onProcess;

        private Option<Subject<float>> _onPhysicsProcess;

        private Option<Subject<InputEvent>> _onInput;

        private Option<Subject<InputEvent>> _onUnhandledInput;

        private Option<IScheduler> _scheduler;

        private Option<IScheduler> _physicsScheduler;

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        private bool _valid = true;

        protected BaseNode()
        {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
        }

        public BaseNode(string name) : this()
        {
            Ensure.That(name, nameof(name)).IsNotNullOrEmpty();

            Name = name;
        }

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

        public void Collect(IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            _disposables += disposable;
        }

        public override void _Notification(int what)
        {
            base._Notification(what);

            if (what == NotificationPredelete)
            {
                PreDestroy();
            }
        }

        protected virtual void PreDestroy()
        {
            _valid = false;

            _onProcess.Iter(l => l.CompleteAndDispose());
            _onProcess = None;

            _onPhysicsProcess.Iter(l => l.CompleteAndDispose());
            _onPhysicsProcess = None;

            _onInput.Iter(l => l.CompleteAndDispose());
            _onInput = None;

            _onUnhandledInput.Iter(l => l.CompleteAndDispose());
            _onUnhandledInput = None;

            _scheduler.OfType<IDisposable>().Iter(d => d.DisposeQuietly());
            _scheduler = None;

            _physicsScheduler.OfType<IDisposable>().Iter(d => d.DisposeQuietly());
            _physicsScheduler = None;

            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();
        }
    }
}
