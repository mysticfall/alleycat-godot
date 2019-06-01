using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Character
{
    public static class NodeExtensions
    {
        public const string PlayerTag = "Player";

        public static bool IsPlayer(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.OfType<IHumanoid>().Exists(n => n.Spatial.GetGroups().Contains(PlayerTag));
        }

        public static Option<T> FindPlayer<T>(this Node node) where T : class, IHumanoid
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.GetTree().GetNodesInGroup<T>(PlayerTag).HeadOrNone();
        }
    }
}
