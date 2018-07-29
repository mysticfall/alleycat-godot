using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public static class NodeExtensions
    {
        private const string EventTrackerName = "NodeEventTracker";

        private const string IdleSchedulerName = "IdleScheduler";

        private const string PhysicsSchedulerName = "PhysicsScheduler";

        [NotNull]
        public static IObservable<float> OnProcess([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTrackerName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcess(true);

            return tracker.OnProcess;
        }

        [NotNull]
        public static IObservable<float> OnPhysicsProcess([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTrackerName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetPhysicsProcess(true);

            return tracker.OnPhysicsProcess;
        }

        [NotNull]
        public static IObservable<InputEvent> OnInput([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTrackerName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcessInput(true);

            return tracker.OnInput;
        }

        [NotNull]
        public static IObservable<InputEvent> OnUnhandledInput([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTrackerName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcessUnhandledInput(true);
            tracker.SetProcessUnhandledKeyInput(true);

            return tracker.OnUnhandledInput;
        }

        [CanBeNull]
        public static IScheduler GetScheduler([NotNull] this Node node, ProcessMode mode)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            IScheduler scheduler;

            switch (mode)
            {
                case ProcessMode.Physics:
                    scheduler = GetPhysicsScheduler(node);
                    break;
                case ProcessMode.Idle:
                    scheduler = GetIdleScheduler(node);
                    break;
                case ProcessMode.Disable:
                    scheduler = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            return scheduler;
        }

        [NotNull]
        public static IScheduler GetIdleScheduler([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var scheduler = node.GetOrCreateNode(IdleSchedulerName, _ => new NodeScheduler(ProcessMode.Idle));

            Debug.Assert(scheduler != null, "scheduler != null");

            return scheduler;
        }

        [NotNull]
        public static IScheduler GetPhysicsScheduler([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var scheduler = node.GetOrCreateNode(PhysicsSchedulerName, _ => new NodeScheduler(ProcessMode.Physics));

            Debug.Assert(scheduler != null, "scheduler != null");

            return scheduler;
        }
    }
}
