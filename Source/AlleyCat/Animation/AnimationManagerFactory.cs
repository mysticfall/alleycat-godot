using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Animation
{
    public class AnimationManagerFactory : BaseAnimationManagerFactory<AnimationManager>
    {
        protected override Validation<string, AnimationManager> CreateService(
            AnimationPlayer player, ILogger logger)
        {
            return new AnimationManager(player, ProcessMode, this, Active, logger);
        }
    }
}
