using System;
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

            switch (Player.PlaybackProcessMode)
            {
                case AnimationPlayer.AnimationProcessMode.Physics:
                    TreePlayer.PlaybackProcessMode = AnimationTreePlayer.AnimationProcessMode.Physics;
                    break;
                case AnimationPlayer.AnimationProcessMode.Idle:
                    TreePlayer.PlaybackProcessMode = AnimationTreePlayer.AnimationProcessMode.Idle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Advance(float delta)
        {
            TreePlayer.Advance(0);

            base.Advance(delta);
        }

        protected override void ProcessFrames(float delta) => TreePlayer.Advance(delta);
    }
}
