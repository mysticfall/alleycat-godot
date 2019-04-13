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
            ILoggerFactory loggerFactory) : base(key, displayName, defaultValue, loggerFactory)
        {
            UseAlpha = useAlpha;
        }
    }
}
