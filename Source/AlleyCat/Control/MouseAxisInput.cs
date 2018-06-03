using System;
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
        public float Maximum { get; set; } = 0.1f;

        private float _maximumValue;

        public override void _Ready()
        {
            base._Ready();

            CalculateMaximumValue();

            GetViewport().Connect("size_changed", this, nameof(CalculateMaximumValue));
        }

        protected override IObservable<float> CreateRawObservable()
        {
            return this.OnUnhandledInput()
                .Where(_ => _maximumValue > 0)
                .OfType<InputEventMouseMotion>()
                .Select(e => e.Relative)
                .Select(v => GetValue(v) / _maximumValue);
        }

        private void CalculateMaximumValue()
        {
            _maximumValue = GetValue(GetViewport().Size) * Maximum;
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
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
