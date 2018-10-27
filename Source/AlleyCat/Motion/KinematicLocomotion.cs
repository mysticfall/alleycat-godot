using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Setting.Project;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Options;

namespace AlleyCat.Motion
{
    public abstract class KinematicLocomotion : Locomotion<KinematicBody>
    {
        public float Gravity { get; private set; }

        public Vector3 GravityVector { get; private set; }

        public float FallDuration { get; private set; }

        [Export]
        public bool ApplyGravity { get; set; } = true;

        public Physics3DSettings Physics3DSettings => _settings.Map(s=> s.Value).Head();

        [Service] private Option<IOptions<Physics3DSettings>> _settings;

        protected KinematicLocomotion()
        {
            ProcessMode = ProcessMode.Physics;
        }

        [PostConstruct]
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Gravity = Physics3DSettings.DefaultGravity;
            GravityVector = Physics3DSettings.DefaultGravityVector;

            FallDuration = 0;
        }

        protected override void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var effective = KinematicProcess(delta, velocity, rotationalVelocity);

            Debug.Assert(Target != null, "Target != null");

            if (Target.IsOnFloor() || !ApplyGravity)
            {
                FallDuration = 0;
            }
            else
            {
                FallDuration += delta;

                effective += GravityVector * Gravity * FallDuration;
            }

            Target.MoveAndSlide(effective, Vector3.Up);
        }

        protected abstract Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity);
    }
}
