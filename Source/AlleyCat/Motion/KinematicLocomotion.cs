using System.Diagnostics;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Motion
{
    public abstract class KinematicLocomotion : Locomotion<KinematicBody>
    {
        public float Gravity { get; private set; }

        public Vector3 GravityVector { get; private set; }

        public float FallDuration { get; private set; }

        [Export]
        public bool ApplyGravity { get; set; } = true;

        protected override ProcessMode ProcessMode { get; } = ProcessMode.Physics;

        public override void _Ready()
        {
            base._Ready();

            Gravity = (float) ProjectSettings.GetSetting("physics/3d/default_gravity");
            GravityVector = (Vector3) ProjectSettings.GetSetting("physics/3d/default_gravity_vector");

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

            Target.MoveAndSlide(effective);
        }

        protected abstract Vector3 KinematicProcess(float delta, Vector3 velocity, Vector3 rotationalVelocity);
    }
}
