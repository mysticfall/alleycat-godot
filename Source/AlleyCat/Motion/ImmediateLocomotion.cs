using AlleyCat.Event;
using Godot;

namespace AlleyCat.Motion
{
    public class ImmediateLocomotion : Locomotion<Spatial>
    {
        public override ProcessMode ProcessMode { get; }

        public ImmediateLocomotion(
            Spatial target, 
            ProcessMode processMode, 
            ITimeSource timeSource, 
            bool active = true) : base(target, timeSource, active)
        {
            ProcessMode = processMode;
        }

        protected override void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var axis = rotationalVelocity.Normalized();
            var rotation = rotationalVelocity.Length() * delta;

            Target.RotateObjectLocal(axis, rotation);
            Target.TranslateObjectLocal(velocity * delta);
        }
    }
}
