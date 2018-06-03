using System;
using System.Reactive.Concurrency;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Priority_Queue;

namespace AlleyCat.Event
{
    public class NodeScheduler : Node, IScheduler
    {
        public ProcessMode ProcessMode { get; }

        public DateTimeOffset Now => DateTimeOffset.Now;

        private readonly IPriorityQueue<Task, double> _tasks = new SimplePriorityQueue<Task, double>();

        public NodeScheduler(ProcessMode mode)
        {
            ProcessMode = mode;

            SetProcess(ProcessMode == ProcessMode.Idle);
            SetPhysicsProcess(ProcessMode == ProcessMode.Physics);
        }

        [NotNull]
        public IDisposable Schedule<TState>(
            [CanBeNull] TState state, [NotNull] Func<IScheduler, TState, IDisposable> action) =>
            Schedule(state, Now.Ticks, action);

        [NotNull]
        public IDisposable Schedule<TState>(
            [CanBeNull] TState state,
            DateTimeOffset dueTime,
            [NotNull] Func<IScheduler, TState, IDisposable> action) =>
            Schedule(state, dueTime.Ticks, action);

        [NotNull]
        public IDisposable Schedule<TState>(
            [CanBeNull] TState state,
            TimeSpan dueTime,
            [NotNull] Func<IScheduler, TState, IDisposable> action) =>
            Schedule(state, Now.Ticks + dueTime.Ticks, action);

        private IDisposable Schedule<TState>(
            TState state,
            long dueTime,
            Func<IScheduler, TState, IDisposable> action)
        {
            Ensure.Any.IsNotNull(action, nameof(action));

            var task = new Task(dueTime, () => action(this, state));

            lock (_tasks)
            {
                _tasks.Enqueue(task, task.Ticks);
            }

            return task;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (ProcessMode == ProcessMode.Idle)
            {
                Process();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (ProcessMode == ProcessMode.Physics)
            {
                Process();
            }
        }

        private void Process()
        {
            lock (_tasks)
            {
                var now = Now.Ticks;

                while (_tasks.Count > 0 && _tasks.First.Ticks <= now)
                {
                    _tasks.Dequeue().Execute();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            lock (_tasks)
            {
                _tasks.Clear();
            }
        }

        private class Task : IDisposable
        {
            public long Ticks { get; }

            private readonly Action _action;

            private bool _done;

            public Task(long ticks, Action action)
            {
                Ticks = ticks;

                _action = action;
                _done = false;
            }

            public void Execute()
            {
                if (!_done) _action.Invoke();
            }

            public void Dispose()
            {
                _done = true;
            }
        }
    }
}
