using AlleyCat.Action;
using AlleyCat.Autowire;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public class CreateUIAction : Action.Action
    {
        [Export, UsedImplicitly]
        public PackedScene UI { get; private set; }

        [Node(required: false)]
        public Node Parent { get; private set; }

        public override bool Valid => base.Valid && (UI?.CanInstance() ?? false);

        [Export, UsedImplicitly] private NodePath _parent;

        protected override void DoExecute(IActor actor)
        {
            var instance = UI?.Instance();

            if (instance == null) return;

            var parent = Parent ?? GetTree().CurrentScene;

            parent.AddChild(instance);
        }

        public override bool AllowedFor(IActor context) => true;
    }
}
