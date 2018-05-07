using AlleyCat.Autowire;
using Godot;

namespace AlleyCat.Animation
{
    [Singleton(typeof(IAnimationManager), typeof(IAnimationStateManager))]
    public class AnimationStateManager : AnimationManager, IAnimationStateManager
    {
        [Service]
        public AnimationTreePlayer TreePlayer { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TreePlayer.Active = false;
        }

        public override void Advance(float delta)
        {
            TreePlayer.Advance(0);

            base.Advance(delta);
        }

        protected override void ProcessFrames(float delta) => TreePlayer.Advance(delta);
    }
}
