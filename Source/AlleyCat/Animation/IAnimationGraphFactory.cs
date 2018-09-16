using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationGraphFactory
    {
        AnimationGraph Create(
            [NotNull] string name,
            [CanBeNull] IAnimationGraph parent,
            [NotNull] AnimationGraphContext context);

        AnimationGraph Create(
            [NotNull] AnimationRootNode node,
            [NotNull] AnimationGraphContext context);
    }
}
