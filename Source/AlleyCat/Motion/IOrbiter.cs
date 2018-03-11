using AlleyCat.Common;
using Godot;

namespace AlleyCat.Motion
{
    public interface IOrbiter : IDirectional
    {
        float Pitch { get; set; }

        float Yaw { get; set; }

        float Distance { get; set; }

        void Rotate(Vector2 rotation);
    }
}
