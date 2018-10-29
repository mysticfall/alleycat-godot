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

        public static IObservable<float> OnProcess(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            if (node is BaseNode b) return b.OnProcess;

            var tracker = node.GetComponent(EventTrackerName, _ => new BaseNode(EventTrackerName));

            tracker.SetProcess(true);

            return tracker.OnProcess;
        }

        public static IObservable<float> OnPhysicsProcess(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            if (node is BaseNode b) return b.OnPhysicsProcess;

            var tracker = node.GetComponent(EventTrackerName, _ => new BaseNode(EventTrackerName));

            tracker.SetPhysicsProcess(true);

            return tracker.OnPhysicsProcess;
        }

        public static IScheduler GetScheduler(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            if (node is BaseNode b) return b.Scheduler;

            var tracker = node.GetComponent(EventTrackerName, _ => new BaseNode(EventTrackerName));

            return tracker.Scheduler;
        }

        public static IScheduler GetPhysicsScheduler(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            if (node is BaseNode b) return b.PhysicsScheduler;

            var tracker = node.GetComponent(EventTrackerName, _ => new BaseNode(EventTrackerName));

            return tracker.PhysicsScheduler;
        }

        public static IObservable<InputEvent> OnInput(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = node.GetComponent(EventTrackerName, _ => new BaseNode(EventTrackerName));

            tracker.SetProcessInput(true);

            return tracker.OnInput;
        }

        public static IObservable<InputEvent> OnUnhandledInput(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = node.GetComponent(EventTrackerName, _ => new BaseNode(EventTrackerName));

            tracker.SetProcessUnhandledInput(true);
            tracker.SetProcessUnhandledKeyInput(true);

            return tracker.OnUnhandledInput;
        }
    }
}
