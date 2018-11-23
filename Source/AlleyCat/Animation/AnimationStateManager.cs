using System;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Animation
{
    public class AnimationStateManager : AnimationManager, IAnimationStateManager
    {
        public AnimationTree AnimationTree { get; }

        public string Path => string.Empty;

        public AnimationRootNode Root => _graph.Root;

        protected AnimationGraphContext Context { get; }

        protected IAnimationGraphFactory GraphFactory { get; }

        protected IAnimationControlFactory ControlFactory { get; }

        private readonly IAnimationGraph _graph;

        public AnimationStateManager(
            AnimationPlayer player,
            AnimationTree animationTree,
            Option<IAnimationGraphFactory> graphFactory,
            Option<IAnimationControlFactory> controlFactory,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active,
            ILogger logger) : base(player, processMode, timeSource, active, logger)
        {
            Ensure.That(animationTree, nameof(animationTree)).IsNotNull();

            AnimationTree = animationTree;

            GraphFactory = graphFactory.IfNone(() => new AnimationGraphFactory());
            ControlFactory = controlFactory.IfNone(() => new AnimationControlFactory());

            Context = new AnimationGraphContext(Player, AnimationTree, OnAdvance, GraphFactory, ControlFactory);

            _graph = GraphFactory.TryCreate((AnimationRootNode) AnimationTree.TreeRoot, Context).IfNone(() =>
                throw new ArgumentException(
                    "Failed to create animation graph from the specified animation tree.",
                    nameof(animationTree)));

            AnimationTree.ProcessMode = AnimationTree.AnimationProcessMode.Manual;
        }

        public Option<AnimationNode> FindAnimationNode(string name) => _graph.FindAnimationNode(name);

        public Option<IAnimationGraph> FindGraph(string name) => _graph.FindGraph(name);

        public Option<IAnimationControl> FindControl(string name) => _graph.FindControl(name);

        protected override void ProcessFrames(float delta) => AnimationTree.Advance(delta);
    }
}
