using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationControlFactory
    {
        IAnimationControl Create(
            [NotNull] string name, 
            [NotNull] IAnimationGraph parent,
            [NotNull] AnimationGraphContext context);
    }
}
