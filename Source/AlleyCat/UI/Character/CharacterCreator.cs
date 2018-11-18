using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.View;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    [AutowireContext]
    public class CharacterCreator : AutowiredNode, ICharacterAware<IHumanoid>
    {
        [Service]
        public Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.OnNext(value);
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character.AsObservable();

        [Service(true)]
        protected MorphListPanel MorphListPanel { get; private set; }

        [Service(local: true)]
        protected InspectingView View { get; private set; }

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        public CharacterCreator()
        {
            _character = new BehaviorSubject<Option<IHumanoid>>(None).AddTo(this);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            OnCharacterChange
                .Do(character => View.Pivot = character.OfType<ITransformable>().HeadOrNone())
                .Subscribe(MorphListPanel.Load)
                .AddTo(this);
        }
    }
}
