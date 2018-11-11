using Godot;

namespace AlleyCat.Character.Morph
{
    public abstract class ColorMorphDefinitionFactory<T> : MorphDefinitionFactory<T, Color>
        where T : MorphDefinition<Color>
    {
        [Export]
        public bool UseAlpha { get; set; }
    }
}
