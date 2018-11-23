using AlleyCat.Autowire;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Animation
{
    public class AnimationStateManagerFactory : BaseAnimationManagerFactory<AnimationStateManager>
    {
        [Service]
        public Option<AnimationTree> AnimationTree { get; set; }

        [Service]
        public Option<IAnimationGraphFactory> GraphFactory { get; set; }

        [Service]
        public Option<IAnimationControlFactory> ControlFactory { get; set; }

        protected override Validation<string, AnimationStateManager> CreateService(
            AnimationPlayer player,
            ILogger logger)
        {
            return AnimationTree
                .ToValidation("Missing the animation tree.")
                .Map(animationTree =>
                    new AnimationStateManager(
                        player,
                        animationTree,
                        GraphFactory,
                        ControlFactory,
                        ProcessMode,
                        this,
                        Active,
                        logger));
        }
    }
}
