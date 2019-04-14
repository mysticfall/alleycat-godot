using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public abstract class ColorMorphDefinition : MorphDefinition<Color>
    {
        public bool UseAlpha { get; }

        protected ColorMorphDefinition(
            string key,
            string displayName,
            Color defaultValue,
            bool useAlpha,
            bool hidden,
            ILoggerFactory loggerFactory) : base(key, displayName, defaultValue, hidden, loggerFactory)
        {
            UseAlpha = useAlpha;
        }
    }
}
