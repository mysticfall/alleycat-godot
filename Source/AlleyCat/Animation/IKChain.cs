using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    //FIXME: A temporary workaround until godotengine/godot#21153 gets fixed.
    public class IKChain : SkeletonIK
    {
        [Export]
        public bool IgnoreRotation { get; set; } = true;

        private Skeleton _skeleton;

        private Option<int> _tipIndex;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct(true)]
        protected virtual void OnInitialize()
        {
            var tick = this.FindClosestAncestor<IAnimatable>()
                .Map(a => a.AnimationManager.OnAdvance)
                .MatchObservable(identity, Observable.Empty<float>);

            tick
                .Where(_ => IsRunning() && IgnoreRotation)
                .Subscribe(_ => ResetRotation())
                .DisposeWith(this);

            _skeleton = GetParentSkeleton();
            _tipIndex = _skeleton.FindBone(TipBone);
        }

        protected virtual void ResetRotation()
        {
            var rotation = _tipIndex.Map(_skeleton.GetBoneGlobalPose).Map(p => p.basis);
            var transform = rotation.Map(r => new Transform(r, Target.origin));

            transform.Iter(t => Target = t);
        }
    }
}
