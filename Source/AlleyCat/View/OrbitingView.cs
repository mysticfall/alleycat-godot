using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.View
{
    public abstract class OrbitingView : Orbiter, IView
    {
        [Node(required: false)]
        public virtual Camera Camera { get; private set; }

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Camera != null && Camera.IsCurrent();

        [Export]
        public float InitialDistance { get; set; } = 0.8f;

        protected virtual IObservable<Vector2> ViewInput => _viewInput.AsVector2Input().Where(_ => Valid);

        protected virtual IObservable<float> ZoomInput => _zoomInput.GetAxis().Where(_ => Valid);

        [Export, UsedImplicitly] private NodePath _cameraPath;

        [Node("Rotation")] private InputBindings _viewInput;

        [Node("Zoom")] private InputBindings _zoomInput;

        protected OrbitingView()
        {
        }

        protected OrbitingView(Range<float> yawRange, Range<float> pitchRange) : base(yawRange, pitchRange)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Camera = Camera ?? GetViewport().GetCamera();
            Distance = InitialDistance;

            OnActiveStateChange
                .Do(v => _viewInput.Active = v)
                .Do(v => _zoomInput.Active = v)
                .Subscribe()
                .AddTo(this);

            ViewInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            ZoomInput
                .Subscribe(v => Distance -= v * 0.05f)
                .AddTo(this);

            OnActiveStateChange
                .Where(v => v)
                .Skip(Active ? 1 : 0)
                .Subscribe(_ => Distance = DistanceRange.Min + 0.1f)
                .AddTo(this);
        }
    }
}
