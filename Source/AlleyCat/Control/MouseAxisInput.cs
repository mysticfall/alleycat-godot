using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.Control
{
    public class MouseAxisInput : AxisInput
    {
        [Export]
        public MouseAxis Axis { get; set; }

        [Export(PropertyHint.ExpRange, "0, 1")]
        public float Maximum
        {
            get => _maximum;
            set => _maximum = Mathf.Clamp(value, 0, 1);
        }

        private float _maximum = 0.1f;

        private float _maximumValue;

        public override void _Ready()
        {
            base._Ready();

            _maximumValue = GetValue(GetViewport().Size) * Maximum;
        }

        protected override IObservable<float> CreateRawObservable()
        {
            return this.OnInput()
                .Where(_ => _maximumValue > 0)
                .OfType<InputEventMouseMotion>()
                .Select(e => e.Relative)
                .Select(v => _maximumValue > 0 ? GetValue(v) / _maximumValue : 0);
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
