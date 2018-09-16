using EnsureThat;

namespace AlleyCat.Animation
{
    public class AnimationControlFactory : IAnimationControlFactory
    {
        public virtual IAnimationControl Create(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));
            Ensure.Any.IsNotNull(context, nameof(context));

            IAnimationControl control;

            if ((control = Animator.Create(name, parent, context)) != null) return control;
            if ((control = Blender.Create(name, parent, context)) != null) return control;
            if ((control = Blender2D.Create(name, parent, context)) != null) return control;

            return null;
        }
    }
}
