using Godot;

namespace AlleyCat.Animation
{
    public interface IAnimationStateManager : IAnimationManager, IAnimationGraph
    {
        AnimationTree AnimationTree { get; }
    }
}
