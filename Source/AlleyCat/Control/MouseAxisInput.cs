using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class MouseAxisInput : AxisInput
    {
        public MouseAxis Axis { get; set; }

        public float ViewportRatio
        {
            get => _viewportRatio.Value;
            set => _viewportRatio.OnNext(Mathf.Clamp(value, 0, 1));
        }

        public IObservable<float> OnViewportRatioChange => _viewportRatio.AsObservable();

        protected IObservable<float> OnUnitDistanceChange { get; }

        private readonly BehaviorSubject<float> _viewportRatio;

        public MouseAxisInput(
            string key,
            MouseAxis axis,
            Viewport viewport,
            IInputSource source,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(key, source, timeSource, active, loggerFactory)
        {
            Ensure.That(viewport, nameof(viewport)).IsNotNull();

            Axis = axis;

            _viewportRatio = CreateSubject(0.005f);

            OnUnitDistanceChange = viewport.OnSizeChange()
                .StartWith(viewport.Size)
                .TakeUntil(Disposed.Where(identity))
                .Select(GetValue)
                .CombineLatest(OnViewportRatioChange, (size, ratio) => size * ratio)
                .Where(v => v > 0)
                .Do(v => this.LogDebug("Using unit distance: {}", v));
        }

        protected override IObservable<float> CreateRawObservable()
        {
            return Source.OnInput
                .OfType<InputEventMouseMotion>()
                .Select(e => e.Relative)
                .Select(GetValue)
                .WithLatestFrom(OnUnitDistanceChange, (input, dist) => input / dist)
                .DistinctUntilChanged();
        }

        private float GetValue(Vector2 position)
        {
            switch (Axis)
            {
                case MouseAxis.X:
                    return position.x;
                case MouseAxis.Y:
                    return position.y;
                default:
                    Debug.Fail($"Unknown Axis value: '{Axis}'.");

                    return 0f;
            }
        }
    }
}
