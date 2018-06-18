using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationStateManager : IAnimationManager
    {
        AnimationTreePlayer TreePlayer { get; }

        void Blend([NotNull] Godot.Animation animation, float influence = 1f);

        void Unblend([NotNull] string animation);
    }
}
