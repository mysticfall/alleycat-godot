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

        public static IObservable<bool> OnInitialize(this Node node) => GetReactiveNode(node).Initialized;

        public static IObservable<bool> OnDispose(this Node node) => GetReactiveNode(node).Disposed;

        public static IObservable<float> OnProcess(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = GetReactiveNode(node);

            tracker.SetProcess(true);

            return tracker.OnProcess;
        }

        public static IObservable<float> OnPhysicsProcess(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = GetReactiveNode(node);

            tracker.SetPhysicsProcess(true);

            return tracker.OnPhysicsProcess;
        }

        public static ITimeSource GetTimeSource(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            if (node is ITimeSource source) return source;

            var tracker = GetReactiveNode(node);

            tracker.SetPhysicsProcess(true);

            return tracker;
        }

        public static IScheduler GetScheduler(this Node node) => GetTimeSource(node).Scheduler;

        public static IScheduler GetPhysicsScheduler(this Node node) => GetTimeSource(node).PhysicsScheduler;

        public static IObservable<InputEvent> OnInput(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = GetReactiveNode(node);

            tracker.SetProcessInput(true);

            return tracker.OnInput;
        }

        public static IObservable<InputEvent> OnUnhandledInput(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var tracker = GetReactiveNode(node);

            tracker.SetProcessUnhandledInput(true);
            tracker.SetProcessUnhandledKeyInput(true);

            return tracker.OnUnhandledInput;
        }

        private static ReactiveNode GetReactiveNode(this Node node)
        {
            if (node is ReactiveNode reactiveNode)
            {
                return reactiveNode;
            }

            return node.GetComponent(EventTrackerName, _ => new ReactiveNode(EventTrackerName));
        }
    }
}
