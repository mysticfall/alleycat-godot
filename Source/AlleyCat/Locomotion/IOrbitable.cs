using Godot;

namespace AlleyCat.Locomotion
{
    public interface IOrbitable
    {
        float Pitch { get; set; }

        float Yaw { get; set; }

        float Distance { get; set; }

        Vector3 Pivot { get; }

        Vector3 Up { get; }

        Vector3 Forward { get; }

        void Rotate(Vector2 rotation);
    }
}
