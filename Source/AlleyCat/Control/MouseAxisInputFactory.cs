using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class MouseAxisInputFactory : AxisInputFactory<MouseAxisInput>
    {
        [Export]
        public MouseAxis Axis { get; set; }

        [Export(PropertyHint.ExpRange, "0, 0.1, 0.001")]
        public float ViewportRatio { get; set; } = 0.005f;

        protected override Validation<string, MouseAxisInput> CreateService(ILoggerFactory loggerFactory)
        {
            return new MouseAxisInput(
                GetName(),
                Axis,
                GetViewport(),
                this,
                this,
                Active,
                loggerFactory)
            {
                ViewportRatio = ViewportRatio,
                Sensitivity = Sensitivity,
                Curve = Optional(Curve),
                DeadZone = DeadZone,
                Interpolate = Interpolate,
                WindowSize = WindowSize,
                WindowShift = WindowShift
            };
        }
    }
}
