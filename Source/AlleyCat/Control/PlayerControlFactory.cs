using System.Collections.Generic;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.View;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    [AutowireContext]
    public class PlayerControlFactory : GameNodeFactory<PlayerControl>
    {
        [Export]
        public bool Active { get; set; } = true;

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Service(local: true)]
        public IEnumerable<IPerspectiveView> Perspectives { get; set; } = Seq<IPerspectiveView>();

        [Service]
        public IActionSet Actions { get; set; }

        [Node("Movement")]
        public Option<IInputBindings> MovementInput { get; set; }

        [Service(local: true)]
        public IEnumerable<IInputBindings> Inputs { get; set; }

        [Export] private NodePath _camera;

        protected override Validation<string, PlayerControl> CreateService(ILoggerFactory loggerFactory)
        {
            return new PlayerControl(
                Perspectives,
                Actions,
                MovementInput,
                Optional(Inputs).Flatten().Bind(b => b.Inputs.Values),
                ProcessMode,
                this,
                Active,
                loggerFactory);
        }
    }
}
