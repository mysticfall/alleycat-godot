using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Motion;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.View
{
    public abstract class OrbitingView : Orbiter, IView
    {
        public virtual Camera Camera => _camera.IfNone(GetViewport().GetCamera());

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Camera.IsCurrent();

        protected virtual IObservable<Vector2> RotationInput => _rotationInput
            .Bind(i => i.AsVector2Input())
            .MatchObservable(identity, Observable.Empty<Vector2>)
            .Where(_ => Valid);

        protected virtual IObservable<float> ZoomInput => _zoomInput
            .Bind(i => i.FindAxis())
            .MatchObservable(identity, Observable.Empty<float>)
            .Where(_ => Valid);

        [Node(false)] private Option<Camera> _camera;

        [Export] private NodePath _cameraPath;

        [Node("Rotation", false)] private Option<InputBindings> _rotationInput;

        [Node("Zoom", false)] private Option<InputBindings> _zoomInput;

        protected OrbitingView()
        {
        }

        protected OrbitingView(
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange) : base(yawRange, pitchRange, distanceRange)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            OnActiveStateChange
                .Do(v => _rotationInput.Iter(i => i.Active = v))
                .Do(v => _zoomInput.Iter(i => i.Active = v))
                .Subscribe()
                .AddTo(this);

            RotationInput
                .Select(v => v * 0.05f)
                .Subscribe(v => Rotation -= v)
                .AddTo(this);

            ZoomInput
                .Subscribe(v => Distance -= v * 0.05f)
                .AddTo(this);
        }
    }
}
