using System.Linq;
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

        public static Vector2 ClosestGlobalAxis(this Vector2 axis)
        {
            var elements = new[] {axis.x, axis.y}.Select((value, index) => (value, index)).ToArray();

            var max = elements.Max(t => Mathf.Abs(t.value));

            var values = elements
                .Select(t => (value: max - Mathf.Abs(t.value), t.index))
                .OrderBy(t => t.index)
                .ToArray();

            values[0].value = 1 * Mathf.Sign(elements[values[0].index].value);
            values[1].value = 0;

            var result = values.OrderBy(t => t.index).Select(t => t.value).ToArray();

            return new Vector2(result[0], result[1]);
        }

        public static Vector3 ClosestGlobalAxis(this Vector3 axis)
        {
            var elements = new[] {axis.x, axis.y, axis.z}
                .Select((value, index) => (value, index))
                .ToArray();

            var max = elements.Max(t => Mathf.Abs(t.value));

            var values = elements
                .Select(t => (value: max - Mathf.Abs(t.value), t.index))
                .OrderBy(t => t.value)
                .ToArray();

            values[0].value = 1 * Mathf.Sign(elements[values[0].index].value);
            values[1].value = 0;
            values[2].value = 0;

            var result = values.OrderBy(t => t.index).Select(t => t.value).ToArray();

            return new Vector3(result[0], result[1], result[2]);
        }
    }
}
