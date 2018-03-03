using System.Diagnostics;
using Godot;

namespace AlleyCat.Motion
{
    public class ImmediateLocomotion : Locomotion<Spatial>
    {
        protected override void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity)
        {
            var axis = rotationalVelocity.Normalized();
            var rotation = rotationalVelocity.Length() * delta;

            Debug.Assert(Target != null, "Target != null");

            Target.RotateObjectLocal(axis, rotation);
            Target.TranslateObjectLocal(velocity * delta);
        }
    }
}
