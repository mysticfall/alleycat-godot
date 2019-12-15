using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Console
{
    [AutowireContext, Singleton(typeof(IConsole), typeof(ICommandConsole), typeof(DebugConsole))]
    public class DebugConsoleFactory : DelegateNodeFactory<DebugConsole, Godot.Control>
    {
        [Export]
        public int BufferSize { get; set; } = 300;

        [Node]
        public Option<AnimationPlayer> Player { get; private set; }

        [Node]
        public Option<RichTextLabel> Content { get; private set; }

        [Node]
        public Option<LineEdit> InputField { get; private set; }

        [Export, UsedImplicitly] private NodePath _player = "../AnimationPlayer";

        [Export, UsedImplicitly] private NodePath _content = "../Container/Content";

        [Export, UsedImplicitly] private NodePath _inputField = "../Container/InputPane/Input";

        [Service]
        public IEnumerable<IConsoleCommandProvider> Providers { get; set; }

        protected override Validation<string, DebugConsole> CreateService(
            Godot.Control node, ILoggerFactory loggerFactory)
        {
            return
                from content in Content
                    .ToValidation("Failed to find the content area.")
                from inputField in InputField
                    .ToValidation("Failed to find the input field.")
                select new DebugConsole(
                    Optional(Providers).Flatten(),
                    Player,
                    BufferSize,
                    content,
                    inputField,
                    node,
                    loggerFactory);
        }

        public void Clear() => Service.Iter(s => s.Clear());

        public void Hide() => Service.Iter(s => s.Hide());
    }
}
