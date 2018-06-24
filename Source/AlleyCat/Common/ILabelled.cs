using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface ILabelled : INamed, IMeshObject
    {
        Vector3 LabelPosition { get; }
    }

    public static class LabelledExtensions
    {
        public const string LabelMarker = "Label";

        [CanBeNull]
        public static Marker GetLabelMarker([NotNull] this IMarkable markable)
        {
            Ensure.Any.IsNotNull(markable, nameof(markable));

            markable.Markers.TryGetValue(LabelMarker, out var marker);

            return marker;
        }
    }
}
