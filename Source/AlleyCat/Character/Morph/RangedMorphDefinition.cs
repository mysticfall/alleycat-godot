using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public abstract class RangedMorphDefinition : MorphDefinition<float>
    {
        [Export, UsedImplicitly]
        public float MinValue { get; private set; }

        [Export, UsedImplicitly]
        public float MaxValue { get; private set; } = 1.0f;
    }
}
