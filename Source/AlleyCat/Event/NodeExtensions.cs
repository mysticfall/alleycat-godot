using System;
using System.Diagnostics;
using System.Reactive;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public static class NodeExtensions
    {
        private const string NodeName = "NodeEventTracker";

        [NotNull]
        public static IObservable<float> OnProcess([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(NodeName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcess(true);

            return tracker.OnProcess;
        }

        [NotNull]
        public static IObservable<float> OnPhysicsProcess([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(NodeName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetPhysicsProcess(true);

            return tracker.OnPhysicsProcess;
        }

        [NotNull]
        public static IObservable<InputEvent> OnInput([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(NodeName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcessInput(true);

            return tracker.OnInput;
        }

        [NotNull]
        public static IObservable<InputEvent> OnUnhandledInput([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(NodeName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcessUnhandledInput(true);
            tracker.SetProcessUnhandledKeyInput(true);

            return tracker.OnUnhandledInput;
        }

        [NotNull]
        public static IObservable<Unit> OnDispose([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(NodeName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnDispose;
        }
    }
}
