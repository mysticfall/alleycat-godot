using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class AnimationControlFactory : IAnimationControlFactory
    {
        public virtual Option<IAnimationControl> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            return Animator.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   Blender.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   Blender2D.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   CrossfadingAnimator.TryCreate(name, parent, context).Map(c => (IAnimationControl) c) |
                   None;
        }
    }
}
