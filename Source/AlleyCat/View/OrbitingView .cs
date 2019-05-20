using System;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Logging;
using AlleyCat.Motion;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.View
{
    public abstract class OrbitingView : Orbiter, IView
    {
        public Camera Camera { get; }

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Camera.Current;

        protected virtual IObservable<Vector2> RotationInput { get; }

        protected virtual IObservable<float> ZoomInput { get; }

        private readonly Option<IInputBindings> _rotationInput;

        private readonly Option<IInputBindings> _zoomInput;

        protected OrbitingView(
            Camera camera,
            Option<IInputBindings> rotationInput,
            Option<IInputBindings> zoomInput,
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange,
            float initialDistance,
            Vector3 initialOffset,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(
            yawRange, 
            pitchRange, 
            distanceRange, 
            initialDistance, 
            initialOffset, 
            processMode, 
            timeSource, 
            active,
            loggerFactory)
        {
            Ensure.That(camera, nameof(camera)).IsNotNull();

            Camera = camera;

            RotationInput = rotationInput
                .Bind(i => i.AsVector2Input())
                .MatchObservable(identity, Observable.Empty<Vector2>)
                .Where(_ => Valid);
            ZoomInput = zoomInput
                .Bind(i => i.FindAxis())
                .MatchObservable(identity, Observable.Empty<float>)
                .Where(_ => Valid);

            _rotationInput = rotationInput;
            _zoomInput = zoomInput;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            RotationInput
                .Select(v => v * 0.02f)
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Rotation -= v, this);
            ZoomInput
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Distance -= v * 0.15f, this);

            OnActiveStateChange
                .Do(v => _rotationInput.Iter(i => i.Active = v))
                .Do(v => _zoomInput.Iter(i => i.Active = v))
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(this);
        }
    }
}
