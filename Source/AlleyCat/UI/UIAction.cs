using AlleyCat.Action;
using AlleyCat.Control;
using AlleyCat.Game;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public abstract class UIAction : InputAction
    {
        public const string TagModal = "Modal";

        public bool Modal { get; }

        public Option<IScene> Scene => Node.GetCurrentScene();

        protected Node Node { get; }

        protected UIAction(
            string key,
            string displayName,
            ITriggerInput input,
            Node node,
            bool modal,
            bool active,
            ILoggerFactory loggerFactory) : base(key, displayName, input, active, loggerFactory)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Modal = modal;
            Node = node;
        }

        protected override Option<IActionContext> CreateActionContext() => new ActionContext();

        public override bool AllowedFor(IActionContext context) =>
            !Modal || Node.GetTree().GetNodesInGroup(TagModal).Count == 0;
    }
}
