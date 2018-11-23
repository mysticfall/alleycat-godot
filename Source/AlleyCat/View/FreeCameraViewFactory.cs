using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Motion;
using Godot;
using LanguageExt;

namespace AlleyCat.View
{
    public class FreeCameraViewFactory : TurretLikeFactory<FreeCameraView>
    {
        [Node]
        public Option<IHumanoid> Character { get; set; }

        [Node]
        public Option<Camera> Camera { get; set; }

        [Export(PropertyHint.ExpRange, "1,10")]
        public float MaxDofDistance { get; set; } = 5f;

        [Export(PropertyHint.ExpRange, "1,10")]
        public float FocusRange { get; set; } = 3f;

        [Export(PropertyHint.ExpRange, "10,1000")]
        public float FocusSpeed { get; set; } = 100f;

        [Export] private NodePath _character;

        [Export] private NodePath _camera;

        [Node("Rotation")]
        public Option<IInputBindings> RotationInput { get; set; }

        [Node("Movement")]
        public Option<IInputBindings> MovementInput { get; set; }

        [Node("Toggle")]
        public Option<IInputBindings> ToggleInput { get; set; }

        public FreeCameraViewFactory()
        {
            Active = false;
        }

        protected override Validation<string, FreeCameraView> CreateService(
            Range<float> yawRange, Range<float> pitchRange)
        {
            return new FreeCameraView(
                Camera.IfNone(() => GetViewport().GetCamera()),
                Character | this.FindPlayer<IHumanoid>(),
                RotationInput,
                MovementInput,
                ToggleInput,
                yawRange,
                pitchRange,
                this,
                Active)
            {
                FocusRange = FocusRange,
                FocusSpeed = FocusSpeed,
                MaxDofDistance = MaxDofDistance
            };
        }
    }
}
