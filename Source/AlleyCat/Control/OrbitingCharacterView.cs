using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    [Singleton(typeof(IPerspectiveView), typeof(IThirdPersonView))]
    public class OrbitingCharacterView : OrbitingView, IThirdPersonView
    {
        public virtual IHumanoid Character { get; set; }

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Character != null;

        public bool AutoActivate => true;

        public override Vector3 Origin => Character?.Vision.Head.origin ?? Vector3.Zero;

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward => Character == null
            ? Vector3.Forward
            : new Plane(Vector3.Up, 0f).Project(Character.GlobalTransform().Forward());

        [Export, UsedImplicitly] private NodePath _characterPath;

        public OrbitingCharacterView() : base(new Range<float>(-180f, 180f), new Range<float>(-89f, 90f))
        {
        }

        public OrbitingCharacterView(Range<float> yawRange, Range<float> pitchRange) : base(yawRange, pitchRange)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ZoomInput
                .Where(_ => Distance <= DistanceRange.Min)
                .Where(v => v > 0)
                .Subscribe(_ => this.Deactivate())
                .AddTo(this);
        }
    }
}
