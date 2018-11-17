using System;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    [AutowireContext]
    public class Scene : AutowiredNode, IScene
    {
        public string Key => _key.TrimToOption().IfNone(GetName);

        public Node Root => this;

        public Node CharactersRoot => Optional(_charactersPath).Bind(Root.FindComponent<Node>).IfNone(Root);

        public Node ItemsRoot => Optional(_itemsPath).Bind(Root.FindComponent<Node>).IfNone(Root);

        public Node UIRoot => Optional(_uiPath).Bind(Root.FindComponent<Node>).IfNone(Root);

        [Export] private string _key;

        [Export] private NodePath _charactersPath = "Characters";

        [Export] private NodePath _itemsPath = "Items";

        [Export] private NodePath _uiPath = "UI";

        public PackedScene Pack() => throw new NotImplementedException();
    }
}
