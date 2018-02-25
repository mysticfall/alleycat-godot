using System.Text;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class VectorExtensions
    {
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
