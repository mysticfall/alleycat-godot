using Godot;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Common
{
    public static class TransformExtensions
    {
        public static Vector3 Up(this Transform transform) => transform.Xform(Axis.Up);

        public static Vector3 Down(this Transform transform) => transform.Xform(Axis.Down);

        public static Vector3 Forward(this Transform transform) => transform.Xform(Axis.Forward);

        public static Vector3 Backward(this Transform transform) => transform.Xform(Axis.Backward);

        public static Vector3 Right(this Transform transform) => transform.Xform(Axis.Right);

        public static Vector3 Left(this Transform transform) => transform.Xform(Axis.Left);
    }
}
