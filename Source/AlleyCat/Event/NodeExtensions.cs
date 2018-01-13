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
        [CanBeNull]
        public static IObservable<Unit> OnReady([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTracker.DefaultName, _ => new EventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnReady;
        }

        [CanBeNull]
        public static IObservable<float> OnProcess([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTracker.DefaultName, _ => new EventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcess(true);

            return tracker.OnProcess;
        }

        [CanBeNull]
        public static IObservable<float> OnPhysicsProcess([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTracker.DefaultName, _ => new EventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetPhysicsProcess(true);

            return tracker.OnPhysicsProcess;
        }

        [CanBeNull]
        public static IObservable<InputEvent> OnInput([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTracker.DefaultName, _ => new EventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            tracker.SetProcessInput(true);

            return tracker.OnInput;
        }

        [CanBeNull]
        public static IObservable<Unit> OnDispose([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(EventTracker.DefaultName, _ => new EventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnDispose;
        }
    }
}
