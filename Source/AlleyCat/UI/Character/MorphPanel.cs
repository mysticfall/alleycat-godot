using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Character
{
    public abstract class MorphPanel : UIControl
    {
        public IMorph Morph { get; }

        protected Label Label { get; }

        protected MorphPanel(
            IMorph morph,
            Label label,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(morph, nameof(morph)).IsNotNull();
            Ensure.That(label, nameof(label)).IsNotNull();

            Morph = morph;
            Label = label;
        }
    }

    namespace Generic
    {
        public abstract class MorphPanel<TVal, TDef> : MorphPanel where TDef : IMorphDefinition
        {
            public new IMorph<TVal, TDef> Morph { get; }

            protected MorphPanel(
                IMorph<TVal, TDef> morph, 
                Label label, 
                Godot.Control node, 
                ILoggerFactory loggerFactory) : base(morph, label, node, loggerFactory)
            {
                Ensure.That(morph, nameof(morph)).IsNotNull();
                Ensure.That(label, nameof(label)).IsNotNull();

                Morph = morph;
            }
        }
    }
}
