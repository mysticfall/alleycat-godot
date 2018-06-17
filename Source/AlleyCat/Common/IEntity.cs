using AlleyCat.IO;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IEntity : INamed, IStateHolder, IMarkable, IHideable, IValidatable
    {
        Vector3 LabelPosition { get; }
    }

    public static class EntityExtensions
    {
        public const string LabelMarker = "Label";

        [CanBeNull]
        public static Marker GetLabelMarker([NotNull] this IEntity entity)
        {
            Ensure.Any.IsNotNull(entity, nameof(entity));

            entity.Markers.TryGetValue(LabelMarker, out var marker);

            return marker;
        }

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
