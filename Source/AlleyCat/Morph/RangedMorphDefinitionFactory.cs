using Godot;

namespace AlleyCat.Morph
{
    public abstract class RangedMorphDefinitionFactory<T> : MorphDefinitionFactory<T, float>
        where T : MorphDefinition<float>
    {
        [Export]
        public float MinValue { get; set; }

        [Export]
        public float MaxValue { get; set; } = 1.0f;
    }
}
