using Godot;

namespace AlleyCat.Animation
{
    public interface IAnimationStateManager : IAnimationManager
    {
        AnimationTreePlayer TreePlayer { get; }
    }
}
