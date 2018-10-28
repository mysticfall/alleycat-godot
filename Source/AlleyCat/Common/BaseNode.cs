using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Common
{
    public class BaseNode : Node, IDisposableCollector, ITimeSource, IValidatable
    {
        public virtual bool Valid => true;

        public IObservable<float> OnIdleLoop => _onIdleLoop.IfNone(() =>
        {
            SetProcess(true);

            return (_onIdleLoop = new Subject<float>()).Head();
        });

        public IObservable<float> OnPhysicsLoop => _onPhysicsLoop.IfNone(() =>
        {
            SetPhysicsProcess(true);

            return (_onPhysicsLoop = new Subject<float>()).Head();
        });

        public IScheduler IdleScheduler => this.GetIdleScheduler();

        public IScheduler PhysicsScheduler => this.GetPhysicsScheduler();

        private Option<Subject<float>> _onIdleLoop;

        private Option<Subject<float>> _onPhysicsLoop;

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        protected BaseNode()
        {
            SetProcess(false);
            SetPhysicsProcess(false);
        }

        public BaseNode(string name) : this()
        {
            Ensure.That(name, nameof(name)).IsNotNullOrEmpty();

            Name = name;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            _onIdleLoop.Iter(l => l.OnNext(delta));
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _onPhysicsLoop.Iter(l => l.OnNext(delta));
        }

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
                OnPreDestroy();
            }
        }

        protected virtual void OnPreDestroy()
        {
            _onIdleLoop.Iter(l => l.CompleteAndDispose());
            _onPhysicsLoop.Iter(l => l.CompleteAndDispose());

            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();
        }
    }
}
