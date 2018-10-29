using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Character
{
    public static class NodeExtensions
    {
        public const string PlayerTag = "Player";

        public static Option<T> FindPlayer<T>(this Node node) where T : ICharacter
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.GetTree().GetNodesInGroup<T>(PlayerTag).HeadOrNone();
        }
    }
}
