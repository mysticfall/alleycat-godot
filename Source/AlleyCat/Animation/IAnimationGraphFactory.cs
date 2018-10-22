using Godot;
using LanguageExt;

namespace AlleyCat.Animation
{
    public interface IAnimationGraphFactory
    {
        Option<IAnimationGraph> TryCreate(string name, IAnimationGraph parent, AnimationGraphContext context);

        Option<IAnimationGraph> TryCreate(AnimationRootNode node, AnimationGraphContext context);
    }
}
