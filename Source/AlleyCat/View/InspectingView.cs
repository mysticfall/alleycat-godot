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

                    return (bounds.Position + bounds.End) / 2f;
                }

                return Pivot?.Spatial.GlobalTransform.origin ?? Vector3.Zero;
            }
        }

        public override Range<float> DistanceRange => new Range<float>(_minDistance, _maxDistance);

        public override float InitialDistance
        {
            get
            {
                if (!(Pivot is IBounded bounded))
                {
                    return base.InitialDistance;
                }

                var bounds = bounded.Bounds;

                var fov = (Target as Camera)?.Fov ?? 70f;
                var height = bounds.GetLongestAxisSize();

                var distance = height / 2f / Math.Tan(Mathf.Deg2Rad(fov / 2f));

                return (float) distance + 0.2f;
            }
            set => base.InitialDistance = value;
        }

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward => Pivot == null
            ? Vector3.Back
            : new Plane(Vector3.Up, 0f).Project(Pivot.GlobalTransform().Backward());

        protected override IObservable<Vector2> RotationInput => base.RotationInput.Where(_ => _rotating.Value);

        protected virtual IObservable<Vector2> PanInput =>
            _panInput?.AsVector2Input().Where(_ => Valid && _panning.Value) ?? Observable.Empty<Vector2>();

        [Export, UsedImplicitly] private NodePath _pivot = "../..";

        [Export, UsedImplicitly] private float _minDistance = 0.2f;

        [Export, UsedImplicitly] private float _maxDistance = 3f;

        [Export, UsedImplicitly] private string _rotationModifier = "point";

        [Export, UsedImplicitly] private string _panningModifier = "point2";

        [Node("Pan", false)] private InputBindings _panInput;

        private ReactiveProperty<bool> _rotating;

        private ReactiveProperty<bool> _panning;

        public InspectingView()
        {
        }

        public InspectingView(
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange) : base(yawRange, pitchRange, distanceRange)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Input.SetMouseMode(Input.MouseMode.Visible);

            var input = this.OnUnhandledInput().Where(e => Active && !e.IsEcho());

            ReactiveProperty<bool> ObserveModifier(string name)
            {
                var pressed = input.Select(e => e.IsActionPressed(name)).Where(v => v);
                var released = input.Select(e => e.IsActionReleased(name)).Where(v => v).Select(v => !v);

                return pressed.Merge(released).ToReactiveProperty();
            }

            _rotating = ObserveModifier(_rotationModifier);

            IObservable<bool> interacting = _rotating;

            if (_panInput != null)
            {
                OnActiveStateChange
                    .Do(v => _panInput.Active = v)
                    .Subscribe()
                    .AddTo(this);

                PanInput
                    .Where(_ => _panning.Value)
                    .Select(v => v * 0.05f)
                    .Subscribe(v => Offset += new Vector3(-v.x, v.y, 0))
                    .AddTo(this);

                _panning = ObserveModifier(_panningModifier);

                interacting = interacting.CombineLatest(_panning, (r, p) => r || p);
            }

            interacting
                .Select(v => v ? Input.MouseMode.Captured : Input.MouseMode.Visible)
                .Subscribe(Input.SetMouseMode)
                .AddTo(this);
        }

        protected override void OnPreDestroy()
        {
            _rotating?.Dispose();
            _rotating = null;

            _panning?.Dispose();
            _panning = null;

            base.OnPreDestroy();
        }
    }
}
