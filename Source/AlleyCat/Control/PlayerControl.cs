using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;
using static AlleyCat.Control.PlayerPerspective;

namespace AlleyCat.Control
{
    public class PlayerControl : Orbiter
    {
        [Node]
        public IHumanoid Character { get; set; }

        public PlayerPerspective Perspective
        {
            get => _perspective.Value;
            set
            {
                switch (value)
                {
                    case FirstPerson:
                        if (_perspective.Value != FirstPerson)
                        {
                            Pitch = 0;
                            Yaw = 0;
                            Distance = MinimumDistance;
                        }

                        break;
                    case ThirdPerson:
                        if (_perspective.Value != ThirdPerson)
                        {
                            Pitch = 0;
                            Yaw = 0;
                            Distance = DefaultDistance > MinimumDistance ? Distance : MinimumDistance + 1f;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public IObservable<PlayerPerspective> OnPerspectiveChange => _perspective;

        [Node]
        public InputBindings Movement { get; private set; }

        [Node]
        public InputBindings Rotation { get; private set; }

        [Node]
        public InputBindings Zoom { get; private set; }

        [Export] public float FirstPersonOffset = 0.2f;

        [Export] public float DefaultDistance = 1f;

        public override Vector3 Origin => Character.Vision.Head.origin;

        public override Vector3 Up => Perspective == ThirdPerson ? Axis.Up : Character.Vision.Head.Up();

        public override Vector3 Forward =>
            Perspective == ThirdPerson
                ? new Plane(Axis.Up, 0f).Project(Character.Skeleton.GlobalTransform.Forward())
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

        [Export, UsedImplicitly] private NodePath _character = "..";

        private readonly ReactiveProperty<PlayerPerspective> _perspective;

        public PlayerControl()
        {
            _perspective = new ReactiveProperty<PlayerPerspective>();
        }

        [PostConstruct]
        private void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Distance = DefaultDistance;

            Movement
                .AsVector2Input()
                .Where(_ => Active && Character?.Locomotion != null)
                .Select(v => new Vector3(v.x, 0, -v.y))
                .Subscribe(v => Character.Locomotion.Move(v))
                .AddTo(this);

            Rotation
                .AsVector2Input()
                .Where(_ => Active)
                .Select(v => v * 0.05f)
                .Subscribe(Rotate)
                .AddTo(this);

            Zoom
                .GetAxis("Value")
                .Where(_ => Active)
                .Subscribe(v => Distance -= v * 0.05f)
                .AddTo(this);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!Active) return;

            _perspective.Value = Distance > MinimumDistance ? ThirdPerson : FirstPerson;

            if (Character.Locomotion.Velocity.LengthSquared() < 0.1f)
            {
                Character.Locomotion.Rotate(Axis.Zero);

                return;
            }

            var offset = Mathf.Lerp(0, Yaw, delta * 1.5f);

            Character.Locomotion.Rotate(Axis.Up * offset);
            Yaw -= offset;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _perspective.Dispose();
        }
    }
}
