using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    [AutowireContext]
    public class SceneFactory : GameObjectFactory<Scene>
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public NodePath CharactersPath { get; set; } = "Characters";

        [Export]
        public NodePath ItemsPath { get; set; } = "Items";

        [Export]
        public NodePath UIPath { get; set; } = "UI";

        protected override Validation<string, Scene> CreateService(ILoggerFactory loggerFactory)
        {
            return new Scene(
                Key.TrimToOption().IfNone(() => Name),
                this,
                Optional(CharactersPath),
                Optional(ItemsPath),
                Optional(UIPath),
                loggerFactory);
        }
    }
}
