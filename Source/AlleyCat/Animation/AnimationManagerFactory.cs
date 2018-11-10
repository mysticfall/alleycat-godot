using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Animation
{
    public class AnimationManagerFactory : BaseAnimationManagerFactory<AnimationManager>
    {
        protected override Validation<string, AnimationManager> CreateService(AnimationPlayer player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return new AnimationManager(player, ProcessMode, this, Active);
        }
    }
}
