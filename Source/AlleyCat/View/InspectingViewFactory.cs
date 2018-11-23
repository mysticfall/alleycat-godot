using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.View
{
    public class InspectingViewFactory : OrbitingViewFactory<InspectingView>
    {
        [Export]
        public NodePath Pivot { get; set; } = "../..";

        [Export]
        public string RotationModifier { get; set; } = "point";

        [Export]
        public string PanningModifier { get; set; } = "point2";

        [Node("Pan")]
        public Option<IInputBindings> PanInput { get; set; }

        public InspectingViewFactory()
        {
            MinDistance = 0.2f;
            MaxDistance = 3f;
            InitialDistance = 0.8f;
        }

        protected override Validation<string, InspectingView> CreateService(
            Range<float> yawRange, 
            Range<float> pitchRange, 
            Range<float> distanceRange, 
            ILogger logger)
        {
            var pivot = Optional(Pivot).Bind(this.FindComponent<ITransformable>);

            return new InspectingView(
                pivot,
                Camera.IfNone(() => GetViewport().GetCamera()),
                RotationInput,
                ZoomInput,
                PanInput,
                RotationModifier.TrimToOption(),
                PanningModifier.TrimToOption(),
                yawRange,
                pitchRange,
                distanceRange,
                InitialDistance,
                InitialOffset,
                ProcessMode,
                this,
                this,
                Active,
                logger);
        }
    }
}
