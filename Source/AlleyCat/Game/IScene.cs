using EnsureThat;
using Godot;

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
        public static IScene GetCurrentScene(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return (IScene) node.GetTree().CurrentScene;
        }
    }
}
