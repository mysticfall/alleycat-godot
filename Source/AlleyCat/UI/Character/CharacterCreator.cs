using System;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.View;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    [AutowireContext]
    public class CharacterCreator : AutowiredNode, ICharacterAware<IMorphableCharacter>
    {
        public Option<IMorphableCharacter> Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<Option<IMorphableCharacter>> OnCharacterChange => _character;

        protected MorphListPanel MorphListPanel => _morphListPanel.Head();

        protected InspectingView View=> _view.Head();

        protected Viewport Viewport => _viewportNode.Head();

        [Service] private Option<MorphListPanel> _morphListPanel = None;

        [Node("Control/View")] private Option<InspectingView> _view = None;

        [Export, UsedImplicitly] private NodePath _viewport = "UI/Content Panel/Viewport";

        private readonly ReactiveProperty<Option<IMorphableCharacter>> _character;

        private Option<Viewport> _viewportNode = None;

        public CharacterCreator()
        {
            _character = new ReactiveProperty<Option<IMorphableCharacter>>(None).AddTo(this);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _viewportNode = Optional(_viewport).Bind(this.FindComponent<Viewport>);

            View.Reset();
        }
    }
}
