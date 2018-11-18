using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Game
{
    public interface IScene
    {
        PackedScene Pack();

        Node Root { get; }

        Node CharactersRoot { get; }

        Node ItemsRoot { get; }

        Node UIRoot { get; }
    }

    public static class SceneExtensions
    {
        public static Option<IScene> GetCurrentScene(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.GetTree().CurrentScene.OfType<IScene>();
        }
    }
}
