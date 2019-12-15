using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Game;
using AlleyCat.View;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Character
{
    [AutowireContext]
    public class CharacterCreatorFactory : GameNodeFactory<CharacterCreator>
    {
        [Service]
        public Option<IHumanoid> Character { get; set; }

        [Service]
        public Option<MorphListPanel> MorphListPanel { get; set; }

        [Service(local: true)]
        public Option<InspectingView> View { get; set; }

        protected override Validation<string, CharacterCreator> CreateService(ILoggerFactory loggerFactory)
        {
            return
                from morphListPanel in MorphListPanel
                    .ToValidation("Failed to find the morph list panel.")
                from view in View
                    .ToValidation("Failed to find the camera view.")
                select new CharacterCreator(morphListPanel, view, loggerFactory)
                {
                    Character = Character
                };
        }
    }
}
