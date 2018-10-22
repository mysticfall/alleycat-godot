using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class AnimationControlFactory : IAnimationControlFactory
    {
        public virtual Option<IAnimationControl> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            return Animator.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   Blender.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   Blender2D.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   CrossfadingAnimator.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   None;
        }
    }
}
