using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Character.Morph.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public abstract class MorphPanel : Container
    {
        protected Label Label => _label.Head();

        [Node] private Option<Label> _label = None;

        public abstract void LoadMorph(IMorph morph);

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
            public IMorph<TVal, TDef> Morph => _morph.Head();

            private Option<IMorph<TVal, TDef>> _morph = None;

            public override void LoadMorph(IMorph morph)
            {
                Ensure.That(morph, nameof(morph)).IsNotNull();

                _morph = Some(morph).Cast<IMorph<TVal, TDef>>().HeadOrNone();
            }
        }
    }
}
