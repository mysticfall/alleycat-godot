using AlleyCat.Action;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class CreateUIAction : UIAction
    {
        public PackedScene UI => Some(_ui).Head();

        [Node]
        public Option<Node> Parent { get; private set; }

        public override bool Valid => base.Valid && (_ui?.CanInstance() ?? false);

        [Export, UsedImplicitly] private PackedScene _ui;

        [Export] private NodePath _parent;

        protected override void DoExecute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            Parent.IfNone(GetTree().CurrentScene).AddChild(UI.Instance());
        }
    }
}
