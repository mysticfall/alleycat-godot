using System;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    [AutowireContext]
    public class Scene : AutowiredNode, IScene
    {
        public string Key => _key.TrimToOption().IfNone(GetName);

        public NodePath CharactersPath
        {
            get => Optional(_charactersPath).IfNone(GetPath);
            set
            {
                Ensure.That(value, nameof(value)).IsNotNull();

                _charactersPath = value == GetPath() ? null : value;
            }
        }

        public NodePath ItemsPath
        {
            get => Optional(_itemsPath).IfNone(GetPath);
            set
            {
                Ensure.That(value, nameof(value)).IsNotNull();

                _itemsPath = value == GetPath() ? null : value;
            }
        }

        [Export] private string _key;

        [Export] private NodePath _charactersPath;

        [Export] private NodePath _itemsPath;

        public PackedScene Pack() => throw new NotImplementedException();
    }
}
