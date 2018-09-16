using System;
using Godot;
using JetBrains.Annotations;

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
            [NotNull] AnimationPlayer player,
            [NotNull] AnimationTree animationTree,
            [NotNull] IObservable<float> onAdvance,
            [NotNull] IAnimationGraphFactory graphFactory,
            [NotNull] IAnimationControlFactory controlFactory)
        {
            Player = player;
            AnimationTree = animationTree;
            OnAdvance = onAdvance;
            GraphFactory = graphFactory;
            ControlFactory = controlFactory;
        }
    }
}
