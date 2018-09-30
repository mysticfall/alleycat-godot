using System;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimator : IAnimationControl
    {
        Godot.Animation Animation { get; set; }

        IObservable<Godot.Animation> OnAnimationChange { get; }
    }

    public static class AnimatorExtensions
    {
        [CanBeNull]
        public static IAnimator GetAnimator(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.GetDescendantControl(path) as IAnimator;
        }
    }
}
