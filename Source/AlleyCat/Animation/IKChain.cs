using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Animation
{
    //FIXME: A temporary workaround until godotengine/godot#21153 gets fixed.
    public class IKChain : SkeletonIK
    {
        [Export]
        public bool IgnoreRotation { get; set; } = true;

        [Ancestor]
        protected IAnimatable Animatable { get; private set; }

        private Skeleton _skeleton;

        private int _tipIndex;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct(true)]
        protected virtual void OnInitialize()
        {
            Animatable.AnimationManager.OnAdvance
                .Where(_ => IsRunning() && IgnoreRotation)
                .Subscribe(_ => ResetRotation())
                .AddTo(this.GetCollector());

            _skeleton = GetParentSkeleton();
            _tipIndex = _skeleton?.FindBone(TipBone) ?? -1;
        }

        protected virtual void ResetRotation()
        {
            if (_tipIndex < 0) return;

            var rotation = _skeleton.GetBoneGlobalPose(_tipIndex).basis;
            var transform = Target;

            Target = new Transform(rotation, transform.origin);
        }
    }
}
