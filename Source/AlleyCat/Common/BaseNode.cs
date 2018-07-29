using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public class BaseNode : Node, IDisposableCollector, IGameLoopAware
    {
        [Export, UsedImplicitly]
        public ProcessMode ProcessMode { get; protected set; } = ProcessMode.Disable;

        public virtual IObservable<float> OnLoop => _onLoop;

        private readonly Subject<float> _onLoop = new Subject<float>();

        private IList<IDisposable> _disposables;

        public BaseNode()
        {
        }

        public BaseNode([NotNull] string name)
        {
            Ensure.Any.IsNotNull(name, nameof(name));

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

        protected virtual void ProcessLoop(float delta) => _onLoop.OnNext(delta);

        public void Collect(IDisposable disposable)
        {
            Ensure.Any.IsNotNull(disposable, nameof(disposable));

            if (_disposables == null)
            {
                _disposables = new List<IDisposable>();
            }
            else if (_disposables.Contains(disposable))
            {
                return;
            }

            _disposables.Add(disposable);
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
            _onLoop?.OnCompleted();
            _onLoop?.Dispose();

            _disposables?.Where(d => d != null).Reverse().ToList().ForEach(d => d.Dispose());
            _disposables = null;
        }
    }
}
