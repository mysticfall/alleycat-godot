using Godot;

namespace AlleyCat.Setting.Project
{
    [Settings]
    public class PhysicsSettings : IProjectSettings
    {
        public PhysicsCommonSettings Common { get; set; }

        public Physics3DSettings ThreeD { get; set; }
    }

    [Settings("Common")]
    public class PhysicsCommonSettings : IProjectSettings
    {
        public int PhysicsFps { get; set; }

        public bool EnableObjectPicking { get; set; }
    }

    [Settings("ThreeD")]
    public class Physics3DSettings : IProjectSettings
    {
        public string PhysicsEngine { get; set; }

        public float DefaultGravity { get; set; }

        public Vector3 DefaultGravityVector { get; set; }

        public float DefaultLinearDamp { get; set; }

        public float DefaultAngularDamp { get; set; }
    }
}
