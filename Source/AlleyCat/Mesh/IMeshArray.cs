using EnsureThat;
using static Godot.ArrayMesh;

namespace AlleyCat.Mesh
{
    public interface IMeshArray
    {
        uint FormatMask { get; }
    }

    public static class MeshDataExtensions
    {
        public static bool SupportsFormat(this IMeshArray array, ArrayFormat format)
        {
            Ensure.That(array, nameof(array)).IsNotNull();

            return (array.FormatMask & (uint) format) > 0;
        }
    }
}
