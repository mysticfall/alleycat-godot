using AlleyCat.Action;
using AlleyCat.Control;
using Godot;

namespace AlleyCat.UI
{
    public abstract class UIAction : PlayerAction
    {
        public const string TagModal = "Modal";

        [Export]
        public bool Modal { get; set; }

        public override bool AllowedFor(IActionContext context) =>
            !Modal || GetTree().GetNodesInGroup(TagModal).Count == 0;
    }
}
