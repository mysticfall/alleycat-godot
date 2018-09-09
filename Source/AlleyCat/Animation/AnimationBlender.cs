using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Array = Godot.Collections.Array;

namespace AlleyCat.Animation
{
    public class AnimationBlender : IIdentifiable, IDisposable
    {
        public string Key { get; }

        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        [CanBeNull]
        public Godot.Animation Animation
        {
            get => _current;
            set
            {
                AnimationNode.Animation = value?.GetName();

                var filters = new Array();

                if (value != null)
                {
                    var name = value.GetName();

                    if (!Player.HasAnimation(name))
                    {
                        Player.AddAnimation(name, value).ThrowIfNecessary();
                    }

                    FindTransformTracks(value).ToList().ForEach(filters.Add);
                }

                BlendNode.Filters = filters;
                BlendNode.FilterEnabled = filters.Any();

                _current = value;
            }
        }

        public float Amount
        {
            get => _amount.Value;
            set => _amount.Value = value;
        }

        public float TimeScale
        {
            get => TimeScaleNode?.Scale ?? 1f;
            set
            {
                if (TimeScaleNode != null)
                {
                    TimeScaleNode.Scale = value;
                }
            }
        }

        [NotNull]
        protected AnimationPlayer Player { get; }

        [NotNull]
        protected AnimationNodeBlend2 BlendNode { get; }

        [NotNull]
        protected AnimationNodeAnimation AnimationNode { get; }

        [CanBeNull]
        protected AnimationNodeTimeScale TimeScaleNode { get; }

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        private readonly ReactiveProperty<float> _amount = new ReactiveProperty<float>(1f);

        private Godot.Animation _current;

        public AnimationBlender([NotNull] string name, [NotNull] AnimationNodeBlend2 node)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(node, nameof(node));

            //FIXME: A temporary workaround until godotengine/godot#20939 gets fixed.
            Key = name;

            var parent = (AnimationNodeBlendTree) node.GetParent();

            BlendNode = node;

            var input = parent.GetNode(node.GetInputConnection(1));

            switch (input)
            {
                case AnimationNodeTimeScale timeScale:
                    TimeScaleNode = timeScale;
                    AnimationNode = parent.GetNode(timeScale.GetInputConnection(0)) as AnimationNodeAnimation ??
                                    throw new ArgumentException(
                                        "The specified node doesn't have an animation node: " + node.GetName());
                    break;
                case AnimationNodeAnimation animation:
                    AnimationNode = animation;

                    break;
                default:
                    throw new ArgumentException("Unknown node structure: " + node.GetName());
            }

            var tree = BlendNode.GetTree();

            Player = tree.GetNode<AnimationPlayer>(tree.AnimPlayer);

            _active
                .CombineLatest(_amount, (active, amount) => _current != null && active ? amount : 0f)
                .Subscribe(BlendNode.SetAmount);

            _current = AnimationNode.Animation != null ? Player.GetAnimation(AnimationNode.Animation) : null;
        }

        public void Process(float delta)
        {
        }

        public void Dispose()
        {
            _active?.Dispose();
            _amount?.Dispose();
        }

        private static IEnumerable<NodePath> FindTransformTracks(Godot.Animation animation)
        {
            var tracks = animation.GetTrackCount();

            return Enumerable
                .Range(0, tracks)
                .Select(i => (path: animation.TrackGetPath(i), type: animation.TrackGetType(i)))
                .Where(t => t.type == Godot.Animation.TrackType.Transform)
                .Select(t => t.path);
        }
    }
}
