using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public static class StringExtensions
    {
        // TODO Workaround for https://github.com/godotengine/godot/issues/17579
        private const string NullString = "Null";

        public static Option<string> TrimToOption(this string value) =>
            Optional(value).Map(v => v.Trim()).Filter(v => v.Length > 0 && v != NullString);

        public static string TrimToEmpty(this string value) => TrimToOption(value).IfNone("");
    }
}
