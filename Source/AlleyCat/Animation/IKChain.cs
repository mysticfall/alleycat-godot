using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Logging;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    //FIXME: A temporary workaround until godotengine/godot#21153 gets fixed.
    public class IKChain : SkeletonIK, ILoggable
    {
        [Export]
        public bool IgnoreRotation { get; set; } = true;

        [Service, CanBeNull]
        public ILogger Logger { get; private set; }

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
                .Subscribe(_ => ResetRotation(), this);

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
