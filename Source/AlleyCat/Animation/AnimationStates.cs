using System;
using System.Reactive.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class AnimationStates : AnimationGraph
    {
        public new AnimationNodeStateMachine Root { get; }

        public AnimationNodeStateMachinePlayback Playback { get; }

        public string State
        {
            get => Playback.GetCurrentNode();
            set => Playback.Travel(value);
        }

        public IObservable<string> OnStateChange { get; }

        public AnimationStates(
            string path,
            AnimationNodeStateMachine root,
            AnimationGraphContext context) : base(path, root, context)
        {
            Root = root;

            var playbackPath = string.Join("/", "parameters", path, "playback");

            Playback = (AnimationNodeStateMachinePlayback) context.AnimationTree.Get(playbackPath);

            OnStateChange = Context.OnAdvance
                .Select(_ => Playback.GetCurrentNode())
                .DistinctUntilChanged();
        }

        public override AnimationNode GetAnimationNode(string name)
        {
            Ensure.Any.IsNotNull(name, nameof(name));

            return Root.HasNode(name) ? Root.GetNode(name) : null;
        }
    }

    public static class AnimationStatesExtensions
    {
        [CanBeNull]
        public static AnimationStates GetStates(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.GetDescendantGraph(path) as AnimationStates;
        }
    }
}
