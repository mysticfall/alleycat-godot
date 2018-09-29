using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class StringExtensions
    {
        // TODO Workaround for https://github.com/godotengine/godot/issues/17579
        private const string NullString = "Null";

        [CanBeNull]
        public static string TrimToNull([CanBeNull] this string value)
        {
            if (value == null || value == NullString)
            {
                return null;
            }

            var trimmed = value.Trim();

            return trimmed.Length == 0 ? null : trimmed;
        }

        public static string TrimToEmpty([CanBeNull] this string value) => TrimToNull(value) ?? "";
    }
}
