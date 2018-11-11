using Godot;

namespace AlleyCat.Character.Morph
{
    public abstract class ColorMorphDefinition : MorphDefinition<Color>
    {
        public bool UseAlpha { get; }

        protected ColorMorphDefinition(
            string key, 
            string displayName, 
            Color defaultValue,
            bool useAlpha) : base(key, displayName, defaultValue)
        {
            UseAlpha = useAlpha;
        }
    }
}
