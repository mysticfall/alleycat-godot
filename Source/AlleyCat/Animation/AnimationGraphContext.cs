using System;
using EnsureThat;
using Godot;

namespace AlleyCat.Animation
{
    public class AnimationGraphContext
    {
        public AnimationPlayer Player { get; }

        public AnimationTree AnimationTree { get; }

        public IObservable<float> OnAdvance { get; }

        public IAnimationGraphFactory GraphFactory { get; }

        public IAnimationControlFactory ControlFactory { get; }

        public AnimationGraphContext(
            AnimationPlayer player,
            AnimationTree animationTree,
            IObservable<float> onAdvance,
            IAnimationGraphFactory graphFactory,
            IAnimationControlFactory controlFactory)
        {
            Ensure.That(player, nameof(player)).IsNotNull();
            Ensure.That(animationTree, nameof(animationTree)).IsNotNull();
            Ensure.That(onAdvance, nameof(onAdvance)).IsNotNull();
            Ensure.That(graphFactory, nameof(graphFactory)).IsNotNull();
            Ensure.That(controlFactory, nameof(controlFactory)).IsNotNull();

            Player = player;
            AnimationTree = animationTree;
            OnAdvance = onAdvance;
            GraphFactory = graphFactory;
            ControlFactory = controlFactory;
        }
    }
}
