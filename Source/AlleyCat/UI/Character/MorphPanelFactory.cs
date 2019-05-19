using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Game;
using AlleyCat.Morph;
using AlleyCat.Morph.Generic;
using AlleyCat.UI.Character.Generic;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Character
{
    public abstract class MorphPanelFactory<TC, TVal, TDef> : DelegateObjectFactory<TC, Godot.Control>,
        IMorphPanelFactory<TVal, TDef>
        where TC : MorphPanel
        where TDef : IMorphDefinition
    {
        Option<IMorph> IMorphPanelFactory.Morph
        {
            get => _morph.OfType<IMorph>().HeadOrNone();
            set => _morph = value.OfType<IMorph<TVal, TDef>>().HeadOrNone();
        }

        Option<IMorph<TVal, TDef>> IMorphPanelFactory<TVal, TDef>.Morph
        {
            get => _morph;
            set => _morph = value;
        }

        [Node]
        public Option<Label> Label { get; set; }

        [Export, UsedImplicitly] private NodePath _label = "../Label";

        private Option<IMorph<TVal, TDef>> _morph;

        protected override Validation<string, TC> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from morph in _morph
                    .ToValidation("Missing morph instance.")
                from label in Label
                    .ToValidation("Failed to find the label.")
                from panel in CreateService(morph, label, node, loggerFactory)
                select panel;
        }

        protected abstract Validation<string, TC> CreateService(
            IMorph<TVal, TDef> morph,
            Label label,
            Godot.Control node,
            ILoggerFactory loggerFactory);
    }
}
