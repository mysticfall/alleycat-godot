using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Control
{
    public class InspectingViewControl : Orbiter
    {
        [Node]
        public ITransformable Pivot { get; set; }

        [Node]
        public InputBindings Movement { get; private set; }

        [Node]
        public InputBindings Rotation { get; private set; }

        [Node]
        public InputBindings Zoom { get; private set; }

        [Export] public string ControlModifier = "point";

        public override Vector3 Origin => _origin;

        public override Vector3 Up => Axis.Up;

        public override Vector3 Forward =>
            new Plane(Axis.Up, 0f).Project(Pivot.GlobalTransform().Backward());

        [Export, UsedImplicitly] private NodePath _pivot = "../..";

        private Vector3 _origin;

        private bool _modifierPressed;

        [PostConstruct]
        private void OnInitialize()
        {
            if (Pivot is IBounded bounded)
            {
                var bounds = bounded.Bounds;

                var fov = (Target as Camera)?.Fov ?? 70f;
                var height = bounds.GetLongestAxisSize();

                var distance = height / 2f / Math.Tan(Mathf.Deg2Rad(fov / 2f));

                _origin = (bounds.Position + bounds.End) / 2f;

                Distance = (float) distance + 0.2f;
            }
            else
            {
                _origin = Pivot.Spatial.GlobalTransform.origin;
            }

            Movement
                .GetAxis()
                .Where(_ => Active && _modifierPressed)
                .Select(v => v * 0.03f)
                .Subscribe(v => _origin.y += v)
                .AddTo(this);

            Rotation
                .GetAxis()
                .Where(_ => Active && _modifierPressed)
                .Select(v => v * 0.15f)
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
