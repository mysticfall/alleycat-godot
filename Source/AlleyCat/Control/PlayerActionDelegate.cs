using AlleyCat.Action;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public class PlayerActionDelegate : PlayerAction
    {
        public string Action => _action.TrimToOption().Head();

        public override bool Valid => base.Valid && !string.IsNullOrWhiteSpace(_action);

        [Export, UsedImplicitly] private string _action;

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            context.Actor.Bind(a => a.Actions.Find(Action)).Iter(a => a.Execute(context));
        }

        public override bool AllowedFor(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return context.Actor.Bind(a => a.Actions.Find(Action)).Exists(a => a.AllowedFor(context));
        }
    }
}
