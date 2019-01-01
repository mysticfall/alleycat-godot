using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Logging;
using AlleyCat.View;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    [AutowireContext]
    public class CharacterCreator : ReactiveNode, ICharacterAware<IHumanoid>, ILoggable
    {
        [Service]
        public Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.OnNext(value);
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character.AsObservable();

        [Service, CanBeNull]
        public ILogger Logger { get; private set; }

        [Service(true)]
        protected MorphListPanel MorphListPanel { get; private set; }

        [Service(local: true)]
        protected InspectingView View { get; private set; }

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        public CharacterCreator()
        {
            _character = CreateSubject<Option<IHumanoid>>(None);
        }

        [PostConstruct]
        protected virtual void PostConstruct()
        {
            OnCharacterChange
                .Do(character => View.Pivot = character.OfType<ITransformable>().HeadOrNone())
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(MorphListPanel.Load, this);
        }
    }
}
