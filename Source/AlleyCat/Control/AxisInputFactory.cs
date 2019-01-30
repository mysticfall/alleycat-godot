using Godot;

namespace AlleyCat.Control
{
    public abstract class AxisInputFactory<T> : InputFactory<T, float> where T : AxisInput
    {
        [Export(PropertyHint.ExpRange, "0, 10")]
        public float Sensitivity { get; set; } = 1f;

        [Export]
        public Curve Curve { get; set; }

        [Export(PropertyHint.ExpRange, "0, 1")]
        public float DeadZone { get; set; }

        [Export]
        public virtual bool Interpolate { get; set; }

        [Export(PropertyHint.ExpRange, "0, 1000, 5")]
        public float WindowSize { get; set; } = 5f;

        [Export(PropertyHint.ExpRange, "0, 1000, 1")]
        public float WindowShift { get; set; } = 1f;
    }
}
