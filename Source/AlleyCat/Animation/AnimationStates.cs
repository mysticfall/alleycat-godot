using System;
using System.Reactive.Linq;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class AnimationStates : AnimationGraph
    {
        public new AnimationNodeStateMachine Root { get; }

        public AnimationNodeStateMachinePlayback Playback { get; }

        public string State
        {
            get => Playback.GetCurrentNode();
            set
            {
                Ensure.That(value, nameof(value)).IsNotNull();

                Playback.Travel(value);
            }
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

        public override Option<AnimationNode> FindAnimationNode(string name)
        {
            Ensure.Any.IsNotNull(name, nameof(name));

            return Root.HasNode(name) ? Some(Root.GetNode(name)) : None;
        }
    }

    public static class AnimationStatesExtensions
    {
        public static Option<AnimationStates> FindStates(this IAnimationGraph graph, string path)
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();
            Ensure.That(path, nameof(path)).IsNotNull();

            return graph.FindDescendantGraph<AnimationStates>(path);
        }
    }
}
