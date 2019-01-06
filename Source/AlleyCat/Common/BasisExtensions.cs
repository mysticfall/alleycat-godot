using Godot;

namespace AlleyCat.Common
{
    public static class BasisExtensions
    {
        public static Vector3 Up(this Basis basis) => basis.y;

        public static Vector3 Down(this Basis basis) => -basis.y;

        public static Vector3 Forward(this Basis basis) => -basis.z;

        public static Vector3 Backward(this Basis basis) => basis.z;

        public static Vector3 Right(this Basis basis) => basis.x;

        public static Vector3 Left(this Basis basis) => -basis.x;
    }
}
