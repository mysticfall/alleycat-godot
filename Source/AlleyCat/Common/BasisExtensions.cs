using Godot;
using Axis = AlleyCat.Common.VectorExtensions;

namespace AlleyCat.Common
{
    public static class BasisExtensions
    {
        public static Vector3 Up(this Basis basis) => basis.Xform(Axis.Up);

        public static Vector3 Down(this Basis basis) => basis.Xform(Axis.Down);

        public static Vector3 Forward(this Basis basis) => basis.Xform(Axis.Forward);

        public static Vector3 Backward(this Basis basis) => basis.Xform(Axis.Backward);

        public static Vector3 Right(this Basis basis) => basis.Xform(Axis.Right);

        public static Vector3 Left(this Basis basis) => basis.Xform(Axis.Left);
    }
}
