using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Subjects;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public static class NodeExtensions
    {
        public const string NodeName = "NodeEventTracker";

        [NotNull]
        public static IObservable<Unit> OnReady([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(NodeName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnReady;
        }

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
        public static IObservable<Unit> OnDispose([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var tracker = node.GetOrCreateNode(NodeName, _ => new NodeEventTracker());

            Debug.Assert(tracker != null, "tracker != null");

            return tracker.OnDispose;
        }

        public class NodeEventTracker : EventTracker<Node>
        {
            [NotNull]
            public IObservable<Unit> OnReady
            {
                get
                {
                    if (_onReady == null)
                    {
                        _onReady = new Subject<Unit>();
                    }

                    return _onReady;
                }
            }

            [NotNull]
            public IObservable<float> OnProcess
            {
                get
                {
                    if (_onProcess == null)
                    {
                        _onProcess = new Subject<float>();
                    }

                    return _onProcess;
                }
            }

            [NotNull]
            public IObservable<float> OnPhysicsProcess
            {
                get
                {
                    if (_onPhysicsProcess == null)
                    {
                        _onPhysicsProcess = new Subject<float>();
                    }

                    return _onPhysicsProcess;
                }
            }

            [NotNull]
            public IObservable<InputEvent> OnInput
            {
                get
                {
                    if (_onInput == null)
                    {
                        _onInput = new Subject<InputEvent>();
                    }

                    return _onInput;
                }
            }

            [NotNull]
            public IObservable<Unit> OnDispose
            {
                get
                {
                    if (_onDispose == null)
                    {
                        _onDispose = new Subject<Unit>();
                    }

                    return _onDispose;
                }
            }

            private Subject<Unit> _onReady;

            private Subject<float> _onProcess;

            private Subject<float> _onPhysicsProcess;

            private Subject<InputEvent> _onInput;

            private Subject<Unit> _onDispose;

            public override void _Ready()
            {
                base._Ready();

                _onReady?.OnNext(Unit.Default);
            }

            public override void _Process(float delta)
            {
                base._Process(delta);

                _onProcess?.OnNext(delta);
            }

            public override void _PhysicsProcess(float delta)
            {
                base._PhysicsProcess(delta);

                _onPhysicsProcess?.OnNext(delta);
            }

            public override void _Input(InputEvent @event)
            {
                base._Input(@event);

                _onInput?.OnNext(@event);
            }

            protected override void Connect(Node parent)
            {
            }

            protected override void Disconnect(Node parent)
            {
                _onDispose?.OnNext(Unit.Default);
                _onDispose?.Dispose();
                _onDispose = null;

                _onReady?.Dispose();
                _onReady = null;

                _onProcess?.Dispose();
                _onProcess = null;

                _onPhysicsProcess?.Dispose();
                _onPhysicsProcess = null;

                _onInput?.Dispose();
                _onInput = null;
            }
        }
    }
}
