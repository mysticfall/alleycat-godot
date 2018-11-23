using EnsureThat;
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
            Ensure.That(player, nameof(player)).IsNotNull();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return new AnimationManager(player, ProcessMode, this, Active, logger);
        }
    }
}
