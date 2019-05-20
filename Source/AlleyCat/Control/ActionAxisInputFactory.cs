using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class ActionAxisInputFactory : AxisInputFactory<ActionAxisInput>
    {
        [Export]
        public string PositiveAction { get; set; }

        [Export]
        public string NegativeAction { get; set; }

        [Export]
        public bool Polling { get; set; } = true;

        protected override Validation<string, ActionAxisInput> CreateService(ILoggerFactory loggerFactory)
        {
            return
                from positiveAction in PositiveAction.TrimToOption()
                    .ToValidation("Positive action was not specified.")
                from negativeAction in NegativeAction.TrimToOption()
                    .ToValidation("Negative action was not specified.")
                select new ActionAxisInput(
                    Name,
                    positiveAction,
                    negativeAction,
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
                    Polling = Polling
                };
        }
    }
}
