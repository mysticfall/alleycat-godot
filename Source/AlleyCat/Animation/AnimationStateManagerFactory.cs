using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using LanguageExt;

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

        protected override Validation<string, AnimationStateManager> CreateService(AnimationPlayer player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

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
                        Active));
        }
    }
}
