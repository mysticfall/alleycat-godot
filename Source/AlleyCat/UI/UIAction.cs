using AlleyCat.Action;
using AlleyCat.Control;
using Godot;

namespace AlleyCat.UI
{
    public abstract class UIAction : InputAction
    {
        public const string TagModal = "Modal";

        [Export]
        public bool Modal { get; set; }

        protected override IActionContext CreateActionContext() => new ActionContext();

        protected override void DoExecute(IActionContext context)
        {
            throw new System.NotImplementedException();
        }

        public override bool AllowedFor(IActionContext context) =>
            !Modal || GetTree().GetNodesInGroup(TagModal).Count == 0;
    }
}
