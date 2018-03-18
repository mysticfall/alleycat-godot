using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Character.Morph.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Character
{
    public abstract class MorphPanel : Container
    {
        [Node]
        public Label Label { get; private set; }

        public abstract void LoadMorph([NotNull] IMorph morph);

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }

    namespace Generic
    {
        public abstract class MorphPanel<TVal, TDef> : MorphPanel where TDef : IMorphDefinition
        {
            public IMorph<TVal, TDef> Morph { get; private set; }

            public override void LoadMorph(IMorph morph)
            {
                Ensure.Any.IsNotNull(morph, nameof(morph));

                Morph = (IMorph<TVal, TDef>) morph;
            }
        }
    }
}
