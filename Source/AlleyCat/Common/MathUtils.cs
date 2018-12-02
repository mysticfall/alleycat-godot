using Godot;

namespace AlleyCat.Common
{
    public static class MathUtils
    {
        public static float NormalizeAspectAngle(float angle)
        {
            var value = angle;

            while (value < 0) value += 2 * Mathf.Pi;

            return value > Mathf.Pi ? value - 2 * Mathf.Pi : value;
        }
    }
}
