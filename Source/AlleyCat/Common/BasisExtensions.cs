using Godot;

namespace AlleyCat.Common
{
    public static class BasisExtensions
    {
        public static Vector3 Up(this Basis basis) => basis.Xform(Vector3.Up);

        public static Vector3 Down(this Basis basis) => basis.Xform(Vector3.Down);

        public static Vector3 Forward(this Basis basis) => basis.Xform(Vector3.Forward);

        public static Vector3 Backward(this Basis basis) => basis.Xform(Vector3.Back);

        public static Vector3 Right(this Basis basis) => basis.Xform(Vector3.Right);

        public static Vector3 Left(this Basis basis) => basis.Xform(Vector3.Left);
    }
}
