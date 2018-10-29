using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Physics;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using Array = Godot.Collections.Array;

namespace AlleyCat.View
{
    [Singleton(typeof(IPerspectiveView), typeof(IThirdPersonView))]
    public class OrbitingCharacterView : OrbitingView, IThirdPersonView
    {
        [Node(false)]
        public virtual Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character;

        public Option<IEntity> FocusedObject => _focus.Bind(f => f.Value);

        public IObservable<Option<IEntity>> OnFocusChange =>
            _focus.MatchObservable(identity, Observable.Empty<Option<IEntity>>);

        public float MaxFocalDistance
        {
            get => _maxFocalDistance;
            set => _maxFocalDistance = Mathf.Min(value, 0);
        }

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Character.IsSome;

        public bool AutoActivate => true;

        public override Vector3 Origin => Character.Map(c => c.Vision.Head.origin).IfNone(Vector3.Zero);

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward => Character
            .Map(c => c.GlobalTransform().Forward())
            .Map(new Plane(Vector3.Up, 0f).Project)
            .IfNone(Vector3.Forward);

        [Export(PropertyHint.ExpRange, "1,10")]
        private float _maxFocalDistance = 2f;

        [Export] private NodePath _characterPath;

        private readonly ReactiveProperty<Option<IHumanoid>> _character;

        private Option<ReactiveProperty<Option<IEntity>>> _focus;

        public OrbitingCharacterView() : this(
            new Range<float>(-180f, 180f),
            new Range<float>(-89f, 90f),
            new Range<float>(0.25f, 10f))
        {
        }

        public OrbitingCharacterView(
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange) : base(yawRange, pitchRange, distanceRange)
        {
            _character = new ReactiveProperty<Option<IHumanoid>>().AddTo(this);
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
                .Select(to => Character
                    .Map(c => new Array {c})
                    .Bind(v => Camera.GetWorld().IntersectRay(Origin, to, v)))
                .Select(hit => hit.Bind(h => h.Collider.FindEntity()))
                .Select(e => e.Filter(v => v.Valid && v.Visible))
                .DistinctUntilChanged()
                .ToReactiveProperty()
                .AddTo(this);
        }
    }
}
