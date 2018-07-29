using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Character
{
    [AutowireContext]
    public class CharacterCreator : AutowiredNode
    {
        [CanBeNull]
        public IMorphableCharacter Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        [Service]
        protected MorphListPanel MorphListPanel { get; private set; }

        [Node]
        protected Godot.Control Viewport { get; private set; }

        [Export, UsedImplicitly] private NodePath _viewport = "UI/Viewport";

        private readonly ReactiveProperty<IMorphableCharacter> _character;

        public CharacterCreator()
        {
            _character = new ReactiveProperty<IMorphableCharacter>();
        }

        protected override void OnPreDestroy()
        {
            _character.Dispose();

            base.OnPreDestroy();
        }
    }
}
