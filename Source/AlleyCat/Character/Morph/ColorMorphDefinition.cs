using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public abstract class ColorMorphDefinition : MorphDefinition<Color>
    {
        [Export, UsedImplicitly]
        public bool UseAlpha { get; private set; }
    }
}
