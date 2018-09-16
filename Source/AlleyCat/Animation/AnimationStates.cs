using System;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class AnimationStates : AnimationGraph
    {
        public new AnimationNodeStateMachine Root { get; }

        public AnimationNodeStateMachinePlayback Playback { get; }

        public IObservable<string> OnTransition { get; }

        public AnimationStates(
            string path,
            AnimationNodeStateMachine root,
            AnimationGraphContext context) : base(path, root, context)
        {
            Root = root;

            var playbackPath = string.Join("/", "parameters", path, "playback");

            Playback = (AnimationNodeStateMachinePlayback) context.AnimationTree.Get(playbackPath);
        }

        public override AnimationNode GetAnimationNode(string name)
        {
            Ensure.Any.IsNotNull(name, nameof(name));

            return Root.HasNode(name) ? Root.GetNode(name) : null;
        }

        public override void Dispose()
        {
            base.Dispose();
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
