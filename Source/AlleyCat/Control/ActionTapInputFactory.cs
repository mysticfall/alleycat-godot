using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class ActionTapInputFactory : AxisInputFactory<ActionTapInput>
    {
        [Export]
        public string Action { get; set; }

        [Export(PropertyHint.Range, "1,10")]
        public int MaximumTapsPerSecond { get; set; } = 5;

        [Export(PropertyHint.Range, "0.1,5,0.05")]
        public float TapCountingWindow { get; set; } = 0.5f;

        [Export(PropertyHint.Range, "1,100")]
        public int TapCountingResolution { get; set; } = 20;

        protected override Validation<string, ActionTapInput> CreateService(ILoggerFactory loggerFactory)
        {
            return
                from action in Action.TrimToOption()
                    .ToValidation("Action was not specified.")
                select new ActionTapInput(
                    GetName(),
                    action,
                    this,
                    this,
                    Active,
                    loggerFactory)
                {
                    Sensitivity = Sensitivity,
                    Curve = Optional(Curve),
                    DeadZone = DeadZone,
                    Interpolate = Interpolate,
                    WindowSize = WindowSize,
                    WindowShift = WindowShift,
                    MaximumTapsPerSecond = MaximumTapsPerSecond,
                    TapCountingWindow = TapCountingWindow,
                    TapCountingResolution = TapCountingResolution
                };
        }
    }
}
