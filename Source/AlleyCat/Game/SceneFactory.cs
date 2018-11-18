using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
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

        protected override Validation<string, Scene> CreateService()
        {
            return new Scene(
                Key.TrimToOption().IfNone(GetName),
                this,
                Optional(CharactersPath),
                Optional(ItemsPath),
                Optional(UIPath));
        }
    }
}
