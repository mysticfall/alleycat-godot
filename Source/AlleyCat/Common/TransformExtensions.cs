using Godot;

namespace AlleyCat.Common
{
    public static class TransformExtensions
    {
        public static Vector3 Up(this Transform transform) => transform.basis.Up();

        public static Vector3 Down(this Transform transform) => transform.basis.Down();

        public static Vector3 Forward(this Transform transform) => transform.basis.Forward();

        public static Vector3 Backward(this Transform transform) => transform.basis.Backward();

        public static Vector3 Right(this Transform transform) => transform.basis.Right();

        public static Vector3 Left(this Transform transform) => transform.basis.Left();
    }
}
