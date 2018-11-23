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

        public Option<IScene> Scene => _node.GetCurrentScene();

        private readonly Node _node;

        protected UIAction(
            string key,
            string displayName,
            ITriggerInput input,
            Node node,
            bool modal,
            bool active,
            ILogger logger) : base(key, displayName, input, active, logger)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Modal = modal;

            _node = node;
        }

        protected override Option<IActionContext> CreateActionContext() => new ActionContext();

        public override bool AllowedFor(IActionContext context) =>
            !Modal || _node.GetTree().GetNodesInGroup(TagModal).Count == 0;
    }
}
