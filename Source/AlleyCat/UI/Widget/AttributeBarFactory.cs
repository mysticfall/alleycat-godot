using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Widget
{
    public class AttributeBarFactory : AttributeWidgetFactory<AttributeBar>
    {
        [Node]
        public Option<ProgressBar> ProgressBar { get; set; }

        [Export, UsedImplicitly] private NodePath _progressBar = "../Container/Progress";

        protected override Validation<string, AttributeBar> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return ProgressBar
                .ToValidation("Failed to find the progress bar.")
                .Map(progress => new AttributeBar(
                    None,
                    Label,
                    ValueLabel,
                    ValueFormat.TrimToOption(),
                    Icon,
                    progress,
                    node,
                    loggerFactory));
        }
    }
}
