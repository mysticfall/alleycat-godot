using AlleyCat.Autowire;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Tool
{
    public class BlendMapGeneratorFactory : DelegateNodeFactory<BlendMapGenerator, Godot.Control>
    {
        [Node]
        public Option<LineEdit> InputEdit { get; set; }

        [Node]
        public Option<LineEdit> OutputEdit { get; set; }

        [Node]
        public Option<Tree> SourceList { get; set; }

        [Node]
        public Option<Tree> MorphList { get; set; }

        [Node]
        public Option<Label> ProgressLabel { get; set; }

        [Node]
        public Option<ProgressBar> ProgressBar { get; set; }

        [Node]
        public Option<Button> InputButton { get; set; }

        [Node]
        public Option<Button> OutputButton { get; set; }

        [Node]
        public Option<Button> StartButton { get; set; }

        [Node]
        public Option<Button> CloseButton { get; set; }

        [Node]
        public Option<FileDialog> FileDialog { get; set; }

        [Node]
        public Option<Label> InfoLabel { get; set; }

        [Export, UsedImplicitly] private NodePath _inputEdit = "../Container/InputPanel/Input";

        [Export, UsedImplicitly] private NodePath _outputEdit = "../Container/OutputPanel/Output";

        [Export, UsedImplicitly] private NodePath _sourceList = "../Container/ListPanel/Sources/List";

        [Export, UsedImplicitly] private NodePath _morphList = "../Container/ListPanel/Morphs/List";

        [Export, UsedImplicitly] private NodePath _progressLabel = "../Container/ProgressPanel/ProgressLabel";

        [Export, UsedImplicitly] private NodePath _progressBar = "../Container/ProgressPanel/ProgressBar";

        [Export, UsedImplicitly] private NodePath _inputButton = "../Container/InputPanel/ChooseButton";

        [Export, UsedImplicitly] private NodePath _outputButton = "../Container/OutputPanel/ChooseButton";

        [Export, UsedImplicitly] private NodePath _startButton = "../Container/ButtonPanel/StartButton";

        [Export, UsedImplicitly] private NodePath _closeButton = "../Container/ButtonPanel/CloseButton";

        [Export, UsedImplicitly] private NodePath _fileDialog = "../FileDialog";

        [Export, UsedImplicitly] private NodePath _infoLabel = "../Container/ButtonPanel/InfoLabel";

        protected override Validation<string, BlendMapGenerator> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from inputEdit in InputEdit
                    .ToValidation("Failed to find the edit control for the input directory.")
                from outputEdit in OutputEdit
                    .ToValidation("Failed to find the edit control for the output directory.")
                from sourceList in SourceList
                    .ToValidation("Failed to find the source item control.")
                from morphList in MorphList
                    .ToValidation("Failed to find the morph item control.")
                from progressLabel in ProgressLabel
                    .ToValidation("Failed to find the progress label.")
                from progressBar in ProgressBar
                    .ToValidation("Failed to find the progress bar.")
                from inputButton in InputButton
                    .ToValidation("Failed to find the button for the input chooser dialog.")
                from outputButton in OutputButton
                    .ToValidation("Failed to find the button for the output chooser dialog.")
                from startButton in StartButton
                    .ToValidation("Failed to find the start button.")
                from closeButton in CloseButton
                    .ToValidation("Failed to find the close button.")
                from fileDialog in FileDialog
                    .ToValidation("Failed to find the file chooser dialog.")
                from infoLabel in InfoLabel
                    .ToValidation("Failed to find the information label.")
                select new BlendMapGenerator(
                    inputEdit,
                    outputEdit,
                    sourceList,
                    morphList,
                    progressLabel,
                    progressBar,
                    inputButton,
                    outputButton,
                    startButton,
                    closeButton,
                    fileDialog,
                    infoLabel,
                    node,
                    loggerFactory);
        }
    }
}
