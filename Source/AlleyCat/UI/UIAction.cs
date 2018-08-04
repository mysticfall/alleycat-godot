using AlleyCat.Action;
using Godot;

namespace AlleyCat.UI
{
    public abstract class UIAction : Action.Action
    {
        public const string TagModal = "Modal";

        [Export]
        public bool Modal { get; set; }

        public override bool AllowedFor(IActor context) =>
            !Modal || GetTree().GetNodesInGroup(TagModal).Count == 0;
    }
}
