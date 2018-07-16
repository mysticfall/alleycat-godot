using Godot;

namespace AlleyCat.Motion
{
    public class Billboard : Spatial
    {
        public override void _Process(float delta)
        {
            base._Process(delta);

            var camera = GetViewport().GetCamera();

            if (camera == null) return;

            LookAt(camera.GlobalTransform.origin, Vector3.Up);
        }
    }
}
