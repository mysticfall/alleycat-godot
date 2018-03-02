using System.Text;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class VectorExtensions
    {
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);

        public static readonly Vector3 Up = new Vector3(0, 1, 0);

        public static readonly Vector3 Down = -Up;

        public static readonly Vector3 Forward = new Vector3(0, 0, -1);

        public static readonly Vector3 Backward = -Forward;

        public static readonly Vector3 Right = new Vector3(1, 0, 0);

        public static readonly Vector3 Left = -Right;

        public static Vector3 Project(this Vector3 vector, Vector3 normal) =>
            normal.Cross(vector.Cross(normal) / normal.Length()) / normal.Length();

        public static string ToFormatString(this Vector2 vector, [NotNull] string format = "###,##0.00")
        {
            Ensure.Any.IsNotNull(format, nameof(format));

            return new StringBuilder()
                .Append("Vector2(")
                .Append(vector.x.ToString(format))
                .Append(", ")
                .Append(vector.y.ToString(format))
                .Append(")")
                .ToString();
        }

        public static string ToFormatString(this Vector3 vector, [NotNull] string format = "###,##0.00")
        {
            Ensure.Any.IsNotNull(format, nameof(format));

            return new StringBuilder()
                .Append("Vector3(")
                .Append(vector.x.ToString(format))
                .Append(", ")
                .Append(vector.y.ToString(format))
                .Append(", ")
                .Append(vector.z.ToString(format))
                .Append(")")
                .ToString();
        }
    }
}
