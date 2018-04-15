using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;
using static AlleyCat.Control.PlayerPerspective;

namespace AlleyCat.Control
{
    public class PlayerControl : OrbitingViewControl
    {
        [Node]
        public IHumanoid Character { get; private set; }

        public PlayerPerspective Perspective
        {
            get => _perspective.Value;
            set => _perspective.Value = value;
        }

        [Export]
        public PlayerPerspective InitialPerspective { get; set; } = ThirdPerson;

        public IObservable<PlayerPerspective> OnPerspectiveChange => _perspective.Where(v => Active);

        [Export]
        public float FirstPersonOffset { get; set; } = 0.2f;

        public override Range<float> YawRange
        {
            get
            {
                var min = Perspective == FirstPerson ? _minFirstPersonRotation : _minThirdPersonRotation;
                var max = Perspective == FirstPerson ? _maxFirstPersonRotation : _maxThirdPersonRotation;

                return new Range<float>(Mathf.Deg2Rad(min.x), Mathf.Deg2Rad(max.x));
            }
        }

        public override Range<float> PitchRange
        {
            get
            {
                var min = Perspective == FirstPerson ? _minFirstPersonRotation : _minThirdPersonRotation;
                var max = Perspective == FirstPerson ? _maxFirstPersonRotation : _maxThirdPersonRotation;

                return new Range<float>(Mathf.Deg2Rad(min.y), Mathf.Deg2Rad(max.y));
            }
        }

        public override Vector3 Origin => Character.Vision.Head.origin;

        public override Vector3 Up => Perspective == ThirdPerson ? Vector3.Up : Character.Vision.Head.Up();

        public override Vector3 Forward =>
            Perspective == ThirdPerson
                ? new Plane(Vector3.Up, 0f).Project(Character.GlobalTransform().Forward())
                : Character.Vision.Forward;

        protected override Transform TargetTransform
        {
            get
            {
                if (Perspective == ThirdPerson)
                {
                    return base.TargetTransform;
                }

                var pivot = Origin;

                var direction = Forward
                    .Rotated(Up, Yaw)
                    .Rotated(Right.Rotated(Up, Yaw), Pitch);

                var transform = Character.Vision.Head.LookingAt(pivot + direction, Up);

                return new Transform(transform.basis, Character.Vision.Origin + direction * FirstPersonOffset);
            }
        }

        protected IObservable<Vector2> MovementInput => _movementInput.AsVector2Input().Where(_ => Active);

        [Export, UsedImplicitly] private NodePath _character = "..";

        [Node("Movement")] private InputBindings _movementInput;

        [Export, UsedImplicitly] private Vector2 _maxFirstPersonRotation = new Vector2(90f, 70f);

        [Export, UsedImplicitly] private Vector2 _minFirstPersonRotation = new Vector2(-90f, -80f);

        [Export, UsedImplicitly] private Vector2 _maxThirdPersonRotation = new Vector2(180f, 70f);

        [Export, UsedImplicitly] private Vector2 _minThirdPersonRotation = new Vector2(-180f, -89f);

        private readonly ReactiveProperty<PlayerPerspective> _perspective;

        public PlayerControl()
        {
            _perspective = new ReactiveProperty<PlayerPerspective>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Input.SetMouseMode(Input.MouseMode.Captured);

            MovementInput
                .Where(_ => Character?.Locomotion != null)
                .Select(v => new Vector3(v.x, 0, -v.y))
                .Subscribe(v => Character.Locomotion.Move(v))
                .AddTo(this);

            OnDistanceChange
                .Select(v => v <= DistanceRange.Min ? FirstPerson : ThirdPerson)
                .Where(v => Perspective != v)
                .Subscribe(v => Perspective = v)
                .AddTo(this);

            Perspective = InitialPerspective;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!Active) return;

            if (Character.Locomotion.Velocity.LengthSquared() < 0.1f)
            {
                Character.Locomotion.Rotate(Vector3.Zero);

                return;
            }

            var offset = Mathf.Lerp(0, Yaw, delta * 1.5f);

            Character.Locomotion.Rotate(Vector3.Up * offset);
            Yaw -= offset;
        }

        protected override void Dispose(bool disposing)
        {
            _perspective?.Dispose();

            base.Dispose(disposing);
        }
    }
}
