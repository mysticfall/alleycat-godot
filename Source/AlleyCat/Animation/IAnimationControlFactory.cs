using LanguageExt;

namespace AlleyCat.Animation
{
    public interface IAnimationControlFactory
    {
        Option<IAnimationControl> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context);
    }
}
