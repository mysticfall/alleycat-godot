using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public abstract class OrbitingView : Orbiter
    {
        [Node(required: false)]
        public virtual Camera Camera { get; set; }

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Camera != null && Camera.IsCurrent();

        [Export]
        public float InitialDistance { get; set; } = 0.8f;

        protected virtual IObservable<Vector2> ViewInput => _viewInput.AsVector2Input().Where(_ => Active && Valid);

        protected virtual IObservable<float> ZoomInput => _zoomInput.GetAxis().Where(_ => Active && Valid);

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

            ViewInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            ZoomInput
                .Subscribe(v => Distance -= v * 0.05f)
                .AddTo(this);

            OnActiveStateChange
                .Where(v => v)
                .Subscribe(_ => Distance = InitialDistance)
                .AddTo(this);
        }
    }
}
