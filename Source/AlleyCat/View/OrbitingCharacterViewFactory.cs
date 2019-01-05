using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.View
{
    public class OrbitingCharacterViewFactory : OrbitingViewFactory<OrbitingCharacterView>
    {
        [Node]
        public Option<IHumanoid> Character { get; set; }

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxFocalDistance { get; set; } = 2f;

        [Export] private NodePath _character;

        public OrbitingCharacterViewFactory()
        {
            ProcessMode = ProcessMode.Idle;

            MinPitch = -89f;
            MaxPitch = 75f;
            MinDistance = 0.25f;
            MaxDistance = 10f;
            InitialDistance = 0.8f;

            Active = false;
        }

        protected override Validation<string, OrbitingCharacterView> CreateService(
            Range<float> yawRange, 
            Range<float> pitchRange, 
            Range<float> distanceRange, 
            ILoggerFactory loggerFactory)
        {
            return new OrbitingCharacterView(
                Camera.IfNone(() => GetViewport().GetCamera()),
                Character | this.FindPlayer<IHumanoid>(),
                RotationInput,
                ZoomInput,
                yawRange,
                pitchRange,
                distanceRange,
                InitialDistance,
                InitialOffset,
                ProcessMode,
                this,
                Active,
                loggerFactory)
            {
                MaxFocalDistance = MaxFocalDistance
            };
        }
    }
}
