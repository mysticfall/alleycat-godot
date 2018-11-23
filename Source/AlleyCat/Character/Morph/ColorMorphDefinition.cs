using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character.Morph
{
    public abstract class ColorMorphDefinition : MorphDefinition<Color>
    {
        public bool UseAlpha { get; }

        protected ColorMorphDefinition(
            string key,
            string displayName,
            Color defaultValue,
            bool useAlpha,
            ILogger logger) : base(key, displayName, defaultValue, logger)
        {
            UseAlpha = useAlpha;
        }
    }
}
