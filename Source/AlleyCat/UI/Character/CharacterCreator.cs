using System;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using AlleyCat.Control;
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

        [Service]
        protected InspectingViewControl ViewControl { get; private set; }

        [Node]
        protected Godot.Control Viewport { get; private set; }

        [Export, UsedImplicitly] private NodePath _viewport = "UI/Viewport";

        private readonly ReactiveProperty<IMorphableCharacter> _character;

        public CharacterCreator()
        {
            _character = new ReactiveProperty<IMorphableCharacter>();
        }

        [PostConstruct]
        private void OnInitialize()
        {
            Viewport
                .OnMouseEnter()
                .Subscribe(_ => ViewControl.Active = true)
                .AddTo(this);
            Viewport
                .OnMouseExit()
                .Subscribe(_ => ViewControl.Active = false)
                .AddTo(this);
        }

        protected override void Dispose(bool disposing)
        {
            _character.Dispose();

            base.Dispose(disposing);
        }
    }
}
