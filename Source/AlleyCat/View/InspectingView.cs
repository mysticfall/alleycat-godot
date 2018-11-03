using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.View
{
    public class InspectingView : OrbitingView
    {
        public override bool Valid => base.Valid && _pivotNode.IsSome;

        public ITransformable Pivot => _pivotNode.Head();

        public override Vector3 Origin
        {
            get
            {
                var center = _pivotNode
                    .OfType<IBounded>()
                    .Map(b => b.Bounds)
                    .Map(b => (b.Position + b.End) / 2f)
                    .HeadOrNone();

                var origin = _pivotNode
                    .Map(p => p.Spatial.GlobalTransform.origin)
                    .HeadOrNone();

                return (center | origin).IfNone(Vector3.Zero);
            }
        }

        public override Range<float> DistanceRange => new Range<float>(_minDistance, _maxDistance);

        public override float InitialDistance
        {
            get
            {
                var bounds = _pivotNode.OfType<IBounded>().Map(b => b.Bounds).HeadOrNone();
                var fov = Optional(Target).OfType<Camera>().Map(c => c.Fov).HeadOrNone().IfNone(70f);
                var height = bounds.Map(b => b.GetLongestAxisSize());
                var distance = height.Map(h => h / 2f / Math.Tan(Mathf.Deg2Rad(fov / 2f)));

                return distance.Map(d => (float) d + 0.2f).IfNone(() => base.InitialDistance);
            }
            set => base.InitialDistance = Mathf.Max(0, value);
        }

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward => _pivotNode
            .Map(p => p.GlobalTransform().Backward())
            .Map(new Plane(Vector3.Up, 0f).Project)
            .IfNone(Vector3.Back);

        protected override IObservable<Vector2> RotationInput =>
            _rotating.MatchObservable(
                rotating => rotating.Select(v => v ? base.RotationInput : Observable.Never<Vector2>()).Switch(),
                Observable.Empty<Vector2>);

        protected virtual IObservable<Vector2> PanInput => _panObserver
            .MatchObservable(identity, Observable.Empty<Vector2>);

        [Export, UsedImplicitly] private NodePath _pivot = "../..";

        [Export] private float _minDistance = 0.2f;

        [Export] private float _maxDistance = 3f;

        [Export] private string _rotationModifier = "point";

        [Export] private string _panningModifier = "point2";

        [Node("Pan", false)] private Option<InputBindings> _panInput;

        private Option<ITransformable> _pivotNode;

        private Option<IObservable<bool>> _rotating;

        private Option<IObservable<bool>> _panning;

        private Option<IObservable<Vector2>> _panObserver;

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
            Input.SetMouseMode(Input.MouseMode.Visible);

            _pivotNode = Optional(_pivot).Bind(this.FindComponent<ITransformable>);

            var input = this.OnUnhandledInput().Where(e => Active && !e.IsEcho());

            IObservable<bool> CreateModifierObserver(string name)
            {
                var pressed = input.Select(e => e.IsActionPressed(name)).Where(identity);
                var released = input.Select(e => e.IsActionReleased(name)).Where(identity).Select(v => !v);

                return pressed.Merge(released);
            }

            _rotating = _rotationModifier.TrimToOption().Map(CreateModifierObserver);
            _panning = _panningModifier.TrimToOption().Map(CreateModifierObserver);

            _panObserver = _panInput.Bind(p => p.AsVector2Input()).Bind(pan =>
                _panning.Map(p => p.Select(v => v ? pan.Where(_ => Valid) : Observable.Never<Vector2>()).Switch())
            );

            PanInput.Select(v => v * 0.05f)
                .Subscribe(v => Offset += new Vector3(-v.x, v.y, 0))
                .AddTo(this);

            OnActiveStateChange
                .Subscribe(v => _panInput.Iter(p => p.Active = v))
                .AddTo(this);

            var interacting =
                from r in _rotating.Map(r => r.StartWith(false))
                from p in _panning.Map(p => p.StartWith(false))
                select r.CombineLatest(p, (o1, o2) => o1 || o2);

            interacting
                .MatchObservable(identity, Observable.Empty<bool>)
                .Select(v => v ? Input.MouseMode.Captured : Input.MouseMode.Visible)
                .Subscribe(Input.SetMouseMode)
                .AddTo(this);

            base.OnInitialize();
        }
    }
}
