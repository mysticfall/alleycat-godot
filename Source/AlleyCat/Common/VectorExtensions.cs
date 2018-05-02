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
            var elements = new[] {axis.x, axis.y}.Select((v, i) => (v, i)).ToArray();

            var max = elements.Max(t => Mathf.Abs(t.Item1));

            var values = elements.Select(t => (max - Mathf.Abs(t.Item1), t.Item2)).OrderBy(t => t.Item1).ToArray();

            values[0].Item1 = 1 * Mathf.Sign(elements[values[0].Item2].Item1);
            values[1].Item1 = 0;

            var result = values.OrderBy(t => t.Item2).Select(t => t.Item1).ToArray();

            return new Vector2(result[0], result[1]);
        }

        public static Vector3 ClosestGlobalAxis(this Vector3 axis)
        {
            var elements = new[] {axis.x, axis.y, axis.z}.Select((v, i) => (v, i)).ToArray();

            var max = elements.Max(t => Mathf.Abs(t.Item1));

            var values = elements.Select(t => (max - Mathf.Abs(t.Item1), t.Item2)).OrderBy(t => t.Item1).ToArray();

            values[0].Item1 = 1 * Mathf.Sign(elements[values[0].Item2].Item1);
            values[1].Item1 = 0;
            values[2].Item1 = 0;

            var result = values.OrderBy(t => t.Item2).Select(t => t.Item1).ToArray();

            return new Vector3(result[0], result[1], result[2]);
        }
    }
}
