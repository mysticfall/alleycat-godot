using System.Diagnostics;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Common
{
    public interface ILabelled : INamed, IMeshObject
    {
        Vector3 LabelPosition { get; }
    }

    public static class LabelledExtensions
    {
        public const string LabelMarker = "Label";

        public static Option<Marker> FindLabelMarker(this IMarkable markable)
        {
            Ensure.That(markable, nameof(markable)).IsNotNull();

            Debug.Assert(markable.Markers != null, "markable.Markers != null");

            return markable.Markers.Find(LabelMarker);
        }
    }
}
