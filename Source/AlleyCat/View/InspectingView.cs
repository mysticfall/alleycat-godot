using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.View
{
    public class InspectingView : OrbitingView
    {
        [Node]
        public ITransformable Pivot { get; private set; }

        public override Vector3 Origin
        {
            get
            {
                if (Pivot is IBounded bounded)
                {
                    var bounds = bounded.Bounds;

                    return (bounds.Position + bounds.End) / 2f + _offset;
                }

                return Pivot == null ? new Vector3() : Pivot.Spatial.GlobalTransform.origin + _offset;
            }
        }

        public override Range<float> DistanceRange => new Range<float>(_minDistance, _maxDistance);

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward => Pivot == null
            ? Vector3.Back
            : new Plane(Vector3.Up, 0f).Project(Pivot.GlobalTransform().Backward());

        protected override IObservable<Vector2> ViewInput =>
            _viewInput
                .GetAxis()
                .Where(_ => _modifierPressed)
                .Select(v => new Vector2(v * 4f, 0));

        protected virtual IObservable<float> MovementInput => _movementInput.GetAxis().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _pivot = "../..";

        [Export, UsedImplicitly] private float _minDistance = 0.2f;

        [Export, UsedImplicitly] private float _maxDistance = 3f;

        [Export, UsedImplicitly] private string _controlModifier = "point";

        [Node("Rotation")] private InputBindings _viewInput;

        [Node("Movement")] private InputBindings _movementInput;

        private Vector3 _offset;

        private bool _modifierPressed;

        public InspectingView()
        {
        }

        public InspectingView(Range<float> yawRange, Range<float> pitchRange) : base(yawRange, pitchRange)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Input.SetMouseMode(Input.MouseMode.Visible);

            MovementInput
                .Where(_ => _modifierPressed)
                .Select(v => v * 0.05f)
                .Subscribe(v => _offset.y += v)
                .AddTo(this);

            this.OnInput()
                .Where(e => !e.IsEcho())
                .Select(e =>
                    e.IsActionPressed(_controlModifier) ||
                    !e.IsActionReleased(_controlModifier) && _modifierPressed)
                .Subscribe(v => _modifierPressed = v)
                .AddTo(this);

            if (Pivot is IBounded bounded)
            {
                var bounds = bounded.Bounds;

                var fov = (Target as Camera)?.Fov ?? 70f;
                var height = bounds.GetLongestAxisSize();

                var distance = height / 2f / Math.Tan(Mathf.Deg2Rad(fov / 2f));

                Distance = (float) distance + 0.2f;
            }
        }
    }
}
