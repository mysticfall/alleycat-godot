using System;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Character
{
    [AutowireContext]
    public class CharacterCreator : AutowiredNode
    {
        [Service]
        public IMorphableCharacter Character { get; private set; }

        [Service]
        protected MorphListPanel MorphListPanel { get; private set; }

        [Service]
        protected CameraControl CameraControl { get; private set; }

        [Node]
        protected Godot.Control Viewport { get; private set; }

        [Export, UsedImplicitly] private NodePath _viewport = "UI/Viewport";

        [PostConstruct]
        private void OnInitialize()
        {
            Viewport
                .OnMouseEnter()
                .Subscribe(_ => CameraControl.Active = true)
                .AddTo(this);
            Viewport
                .OnMouseExit()
                .Subscribe(_ => CameraControl.Active = false)
                .AddTo(this);
        }
    }
}
