using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.View;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    [AutowireContext]
    public class PlayerControlFactory : GameObjectFactory<PlayerControl>
    {
        [Export]
        public bool Active { get; set; } = true;

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Node(false)]
        public Option<IHumanoid> Character { get; set; }

        [Node(false)]
        public Option<Camera> Camera { get; set; }

        [Service]
        public IEnumerable<IPerspectiveView> Perspectives { get; set; } = Seq<IPerspectiveView>();

        [Node("Movement")]
        public Option<InputBindings> MovementInput { get; set; }

        [Export] private NodePath _characterPath;

        [Export] private NodePath _cameraPath;

        protected override Validation<string, PlayerControl> CreateService()
        {
            return new PlayerControl(
                Camera.IfNone(() => GetViewport().GetCamera()),
                Character | this.FindPlayer<IHumanoid>(),
                Perspectives,
                MovementInput,
                ProcessMode,
                this,
                Active);
        }
    }
}
