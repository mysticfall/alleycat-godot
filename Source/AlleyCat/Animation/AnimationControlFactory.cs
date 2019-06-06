using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class AnimationControlFactory : IAnimationControlFactory
    {
        public virtual Option<IAnimationControl> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            IAnimationControl Initialize(IAnimationControl control)
            {
                control.Initialize();

                return control;
            }

            return
                SeekableAnimator.TryCreate(name, parent, context).Map(Initialize) |
                Animator.TryCreate(name, parent, context).Map(Initialize) |
                Blender.TryCreate(name, parent, context).Map(Initialize) |
                Blender2D.TryCreate(name, parent, context).Map(Initialize) |
                CrossfadingAnimator.TryCreate(name, parent, context).Map(Initialize) |
                TimeScale.TryCreate(name, parent, context).Map(Initialize) |
                Trigger.TryCreate(name, parent, context).Map(Initialize) |
                None;
        }
    }
}
