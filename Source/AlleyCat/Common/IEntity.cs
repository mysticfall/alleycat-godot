using AlleyCat.IO;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IEntity : INamed, IStateHolder, IMarkable
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
    }
}
