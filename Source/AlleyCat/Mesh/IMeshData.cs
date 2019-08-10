using EnsureThat;
using Godot;

namespace AlleyCat.Mesh
{
    public interface IMeshData
    {
        uint FormatMask { get; }
    }

    public static class MeshDataExtensions
    {
        public static bool SupportsFormat(this IMeshData data, ArrayMesh.ArrayFormat format)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            return (data.FormatMask & (uint) format) > 0;
        }
    }
}
