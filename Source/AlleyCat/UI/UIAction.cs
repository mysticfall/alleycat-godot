using AlleyCat.Action;
using AlleyCat.Control;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.UI
{
    public abstract class UIAction : InputAction
    {
        public const string TagModal = "Modal";

        [Export]
        public bool Modal { get; set; }

        protected override Option<IActionContext> CreateActionContext() => new ActionContext();

        public override bool AllowedFor(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return !Modal || GetTree().GetNodesInGroup(TagModal).Count == 0;
        }
    }
}
