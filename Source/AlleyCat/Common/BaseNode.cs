using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public class BaseNode : Node, IDisposableCollector, ITimeSource, IValidatable
    {
        [Export]
        public ProcessMode ProcessMode
        {
            get => _processMode;
            protected set
            {
                _processMode = value;

                SetProcess(value == ProcessMode.Idle);
                SetPhysicsProcess(value == ProcessMode.Physics);
            }
        }

        public virtual bool Valid => true;

        public virtual IObservable<float> OnLoop => _onLoop.Head();

        public IScheduler Scheduler => this.GetComponent(SchedulerName, _ => new NodeScheduler(ProcessMode));

        private Option<Subject<float>> _onLoop = Some(_ => new Subject<float>());

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        private ProcessMode _processMode = ProcessMode.Disable;

        private const string SchedulerName = "NodeScheduler";

        public BaseNode()
        {
        }

        public BaseNode(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNullOrEmpty();

            Name = name;
        }

        public override void _Ready()
        {
            base._Ready();

            SetProcess(ProcessMode == ProcessMode.Idle);
            SetPhysicsProcess(ProcessMode == ProcessMode.Physics);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (ProcessMode == ProcessMode.Idle)
            {
                ProcessLoop(delta);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (ProcessMode == ProcessMode.Physics)
            {
                ProcessLoop(delta);
            }
        }

        protected virtual void ProcessLoop(float delta) => _onLoop.Iter(l => l.OnNext(delta));

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
            _onLoop.Iter(l => l.CompleteAndDispose());

            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();
        }
    }
}
