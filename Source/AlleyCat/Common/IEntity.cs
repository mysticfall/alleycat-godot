using AlleyCat.IO;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IEntity : ILabelled, IStateHolder, IValidatable
    {
    }

    public static class EntityExtensions
    {
        [CanBeNull]
        public static IEntity FindEntity([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            if (node is IEntity entity) return entity;

            var parent = node.GetParent();

            if (parent == null || parent == node.GetTree().CurrentScene)
            {
                return null;
            }

            return FindEntity(parent);
        }
    }
}
