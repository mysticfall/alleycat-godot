using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Physics;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.View
{
    [Singleton(typeof(IPerspectiveView), typeof(IThirdPersonView))]
    public class OrbitingCharacterView : OrbitingView, IThirdPersonView
    {
        public virtual IHumanoid Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<IHumanoid> OnCharacterChange => _character;

        public IEntity FocusedObject => _focus?.Value;

        public IObservable<IEntity> OnFocusChange => _focus ?? Observable.Empty<IEntity>();

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxFocalDistance { get; set; } = 2f;

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Character != null;

        public bool AutoActivate => true;

        public override Vector3 Origin => Character?.Vision.Head.origin ?? Vector3.Zero;

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward => Character == null
            ? Vector3.Forward
            : new Plane(Vector3.Up, 0f).Project(Character.GlobalTransform().Forward());

        [Export, UsedImplicitly] private NodePath _characterPath;

        private readonly ReactiveProperty<IHumanoid> _character = new ReactiveProperty<IHumanoid>();

        private ReactiveProperty<IEntity> _focus;

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

            _focus = this.OnPhysicsProcess()
                .Where(_ => Active && Valid)
                .Select(_ => (Origin - Camera.GlobalTransform.origin).Normalized())
                .Select(direction => Origin + direction * MaxFocalDistance)
                .Select(to => Camera.GetWorld().IntersectRay(Origin, to, new object[] {Character}))
                .Select(hit => hit?.Collider?.FindEntity())
                .Select(e => e != null && e.Valid && e.Visible ? e : null)
                .DistinctUntilChanged()
                .ToReactiveProperty();
        }

        protected override void OnPreDestroy()
        {
            _focus?.Dispose();
            _character?.Dispose();

            base.OnPreDestroy();
        }
    }
}
