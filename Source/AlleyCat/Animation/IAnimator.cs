using System;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Animation
{
    public interface IAnimator : IAnimationControl
    {
        Option<Godot.Animation> Animation { get; set; }

        IObservable<Option<Godot.Animation>> OnAnimationChange { get; }
    }

    public static class AnimatorExtensions
    {
        public static Option<IAnimator> FindAnimator(this IAnimationGraph graph, string path)
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();
            Ensure.That(path, nameof(path)).IsNotNull();

            return graph.FindDescendantControl<IAnimator>(path);
        }
    }
}
