using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Motion;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public abstract class OrbitingViewControl : Orbiter
    {
        public Camera Camera { get; private set; }

        [Export]
        public float InitialDistance { get; set; } = 1f;

        public override Spatial Target => Camera;

        protected virtual IObservable<Vector2> ViewInput => _viewInput.AsVector2Input().Where(_ => Active && Valid);

        protected virtual IObservable<float> ZoomInput => _zoomInput.GetAxis().Where(_ => Active && Valid);

        [Export, UsedImplicitly] private NodePath _camera = "..";

        [Node("Rotation")] private InputBindings _viewInput;

        [Node("Zoom")] private InputBindings _zoomInput;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Camera = this.GetNodeOrDefault(_camera, GetViewport().GetCamera());

            ViewInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            ZoomInput
                .Subscribe(v => Distance -= v * 0.05f)
                .AddTo(this);

            Distance = InitialDistance;
        }
    }
}
