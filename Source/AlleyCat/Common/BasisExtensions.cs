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

        public static Basis CreateFromAxes(Vector3 x, Vector3 y, Vector3 z)
        {
            return new Basis
            (
                new Vector3(x.x, y.x, z.x),
                new Vector3(x.y, y.y, z.y),
                new Vector3(x.z, y.z, z.z)
            );
        }
    }
}
