using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class MouseAxisInput : AxisInput
    {
        public MouseAxis Axis { get; set; }

        public float Maximum
        {
            get => _maximum;
            set => _maximum = Mathf.Clamp(value, 0, 1);
        }

        private float _maximum = 0.1f;

        private readonly float _maximumValue;

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

            _maximumValue = GetValue(viewport.Size) * Maximum;
        }

        protected override IObservable<float> CreateRawObservable()
        {
            return Source.OnInput
                .Where(_ => _maximumValue > 0)
                .OfType<InputEventMouseMotion>()
                .Select(e => e.Relative)
                .Select(v => _maximumValue > 0 ? GetValue(v) / _maximumValue : 0)
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
