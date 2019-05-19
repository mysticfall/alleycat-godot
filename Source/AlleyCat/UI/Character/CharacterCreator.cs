using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Logging;
using AlleyCat.View;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class CharacterCreator : GameObject, ICharacterAware<IHumanoid>
    {
        public Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.OnNext(value);
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character.AsObservable();

        protected MorphListPanel MorphListPanel { get; }

        protected InspectingView View { get; }

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        public CharacterCreator(
            MorphListPanel morphListPanel,
            InspectingView view,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(morphListPanel, nameof(morphListPanel)).IsNotNull();
            Ensure.That(view, nameof(view)).IsNotNull();

            MorphListPanel = morphListPanel;
            View = view;

            _character = CreateSubject<Option<IHumanoid>>(None);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnCharacterChange
                .Do(character => View.Pivot = character.OfType<ITransformable>().HeadOrNone())
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(MorphListPanel.Load, this);
        }
    }
}
