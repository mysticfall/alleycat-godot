using Godot;

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
            bool hidden) : base(key, displayName, defaultValue, hidden)
        {
            UseAlpha = useAlpha;
        }
    }
}
