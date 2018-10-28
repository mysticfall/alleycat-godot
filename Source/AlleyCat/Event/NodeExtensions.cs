using System;
using System.Reactive.Concurrency;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Event
{
    public static class NodeExtensions
    {
        private const string EventTrackerName = "NodeEventTracker";

        private const string IdleSchedulerName = "IdleScheduler";

        private const string PhysicsSchedulerName = "PhysicsScheduler";

        public static IObservable<float> OnProcess(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = node.GetComponent(EventTrackerName, _ => new NodeEventTracker());

            tracker.SetProcess(true);

            return tracker.OnProcess;
        }

        public static IObservable<float> OnPhysicsProcess(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = node.GetComponent(EventTrackerName, _ => new NodeEventTracker());

            tracker.SetPhysicsProcess(true);

            return tracker.OnPhysicsProcess;
        }

        public static IObservable<InputEvent> OnInput(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = node.GetComponent(EventTrackerName, _ => new NodeEventTracker());

            tracker.SetProcessInput(true);

            return tracker.OnInput;
        }

        public static IObservable<InputEvent> OnUnhandledInput(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = node.GetComponent(EventTrackerName, _ => new NodeEventTracker());

            tracker.SetProcessUnhandledInput(true);
            tracker.SetProcessUnhandledKeyInput(true);

            return tracker.OnUnhandledInput;
        }

        public static IScheduler GetIdleScheduler(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.GetComponent(IdleSchedulerName, _ => new NodeScheduler(ProcessMode.Idle));
        }

        public static IScheduler GetPhysicsScheduler(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.GetComponent(PhysicsSchedulerName, _ => new NodeScheduler(ProcessMode.Physics));
        }
    }
}
