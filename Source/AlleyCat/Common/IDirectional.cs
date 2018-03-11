using Godot;

namespace AlleyCat.Common
{
    public interface IDirectional
    {
        Vector3 Origin { get; }

        Vector3 Forward { get; }

        Vector3 Up { get; }

        Vector3 Right { get; }
    }
}
