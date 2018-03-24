using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.UI.Character
{
    [Singleton(typeof(CameraControl))]
    public class CameraControl : Orbiter
    {
        [Node]
        public IHumanoid Character { get; set; }

        [Node]
        public InputBindings Movement { get; private set; }

        [Node]
        public InputBindings Rotation { get; private set; }

        [Node]
        public InputBindings Zoom { get; private set; }

        [Export] public string ControlModifier = "point";

        public override Vector3 Origin => _pivot;

        public override Vector3 Up => Axis.Up;

        public override Vector3 Forward =>
            new Plane(Axis.Up, 0f).Project(Character.GlobalTransform().Forward());

        [Export, UsedImplicitly] private NodePath _character = "..";

        private Vector3 _pivot;

        private bool _modifierPressed;

        [PostConstruct]
        private void OnInitialize()
        {
            var fov = (Target as Camera)?.Fov ?? 70f;

            var bounds = Character.Bounds;
            var height = bounds.GetLongestAxisSize();

            var distance = height / 2f / Math.Tan(Mathf.Deg2Rad(fov / 2f));

            _pivot = (bounds.Position + bounds.End) / 2f;

            Distance = (float) distance + 0.2f;
            Yaw = 180;

            Movement
                .GetAxis()
                .Where(_ => Active && _modifierPressed)
                .Select(v => v * 0.05f)
                .Subscribe(v => _pivot.y += v)
                .AddTo(this);
            
            Rotation
                .GetAxis()
                .Where(_ => Active && _modifierPressed)
                .Select(v => v * 0.1f)
                .Select(v => new Vector2(v, 0))
                .Subscribe(Rotate)
                .AddTo(this);

            Zoom
                .GetAxis()
                .Where(_ => Active)
                .Subscribe(v => Distance -= v * 0.05f)
                .AddTo(this);
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (ControlModifier == null) return;

            if (@event.IsActionPressed(ControlModifier))
            {
                _modifierPressed = true;
            }
            else if (@event.IsActionReleased(ControlModifier))
            {
                _modifierPressed = false;
            }
        }
    }
}
