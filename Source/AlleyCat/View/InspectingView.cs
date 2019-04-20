using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.View
{
    public class InspectingView : OrbitingView
    {
        public Option<ITransformable> Pivot
        {
            get => _pivot;
            set
            {
                _pivot = value;

                Reset();
            }
        }

        public override Vector3 Origin
        {
            get
            {
                var center = Pivot
                    .OfType<IBounded>()
                    .Map(b => b.Bounds)
                    .Map(b => (b.Position + b.End) / 2f)
                    .HeadOrNone();

                var origin = Pivot.Map(p => p.Spatial.GlobalTransform.origin);

                return (center | origin).IfNone(Vector3.Zero);
            }
        }

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward
        {
            get
            {
                var backward = Pivot.Map(p => p.GetGlobalTransform().Backward());
                var forward = backward.Map(new Plane(Vector3.Up, 0f).Project);

                return forward.IfNone(Vector3.Forward);
            }
        }

        public IInputSource InputSource { get; }

        public override bool Valid => base.Valid && Pivot.IsSome;

        protected override IObservable<Vector2> RotationInput =>
            _rotating.MatchObservable(
                rotating => rotating.Select(v => v ? base.RotationInput : Observable.Never<Vector2>()).Switch(),
                Observable.Empty<Vector2>);

        protected virtual IObservable<Vector2> PanInput { get; }

        protected Option<string> RotationModifier { get; }

        protected Option<string> PanningModifier { get; }

        private Option<ITransformable> _pivot;

        private readonly Option<IInputBindings> _panInput;

        private readonly Option<IObservable<bool>> _rotating;

        private readonly Option<IObservable<bool>> _panning;

        public InspectingView(
            Option<ITransformable> pivot,
            Camera camera,
            Option<IInputBindings> rotationInput,
            Option<IInputBindings> zoomInput,
            Option<IInputBindings> panInput,
            Option<string> rotationModifier,
            Option<string> panningModifier,
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange,
            float initialDistance,
            Vector3 initialOffset,
            ProcessMode processMode,
            ITimeSource timeSource,
            IInputSource inputSource,
            bool active,
            ILoggerFactory loggerFactory) : base(
            camera,
            rotationInput,
            zoomInput,
            yawRange,
            pitchRange,
            distanceRange,
            CalculateInitialDistance(pivot, camera, initialDistance),
            initialOffset,
            processMode,
            timeSource,
            active,
            loggerFactory)
        {
            Ensure.That(inputSource, nameof(inputSource)).IsNotNull();

            Pivot = pivot;
            InputSource = inputSource;

            RotationModifier = rotationModifier;
            PanningModifier = panningModifier;

            var input = InputSource.OnUnhandledInput.Where(e => Active && !e.IsEcho());

            IObservable<bool> IsModifierPressed(string name)
            {
                var pressed = input.Select(e => e.IsActionPressed(name)).Where(identity);
                var released = input.Select(e => e.IsActionReleased(name)).Where(identity).Select(v => !v);

                return pressed.Merge(released).StartWith(false);
            }

            _rotating = RotationModifier.Map(IsModifierPressed);
            _panning = PanningModifier.Map(IsModifierPressed);

            _panInput = panInput;

            PanInput = (
                    from pan in _panInput.Bind(i => i.AsVector2Input())
                    from modifier in _panning
                    select modifier.Select(v => v ? pan : Observable.Never<Vector2>()).Switch())
                .MatchObservable(identity, Observable.Empty<Vector2>)
                .Where(_ => Valid)
                .Select(v => v * 0.05f);
        }

        protected override void PostConstruct()
        {
            Input.SetMouseMode(Input.MouseMode.Visible);

            PanInput
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Offset += new Vector3(-v.x, v.y, 0), this);

            OnActiveStateChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => _panInput.Exists(p => p.Active = v), this);

            var interacting =
                from a in _rotating
                from p in _panning
                select a.CombineLatest(p, (o1, o2) => o1 || o2);

            interacting
                .MatchObservable(identity, Observable.Empty<bool>)
                .Select(v => v ? Input.MouseMode.Captured : Input.MouseMode.Visible)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(Input.SetMouseMode, this);

            base.PostConstruct();
        }

        private static float CalculateInitialDistance(
            Option<ITransformable> pivot, Camera camera, float defaultValue)
        {
            Ensure.That(camera, nameof(camera)).IsNotNull();

            var bounds = pivot.OfType<IBounded>().Map(b => b.Bounds).HeadOrNone();
            var height = bounds.Map(b => b.GetLongestAxisSize()).Filter(h => h > 0);
            var distance = height.Map(h => h / 2f / Math.Tan(Mathf.Deg2Rad(camera.Fov / 2f)));

            return distance.Map(d => (float) d + 0.2f).IfNone(defaultValue);
        }
    }
}
