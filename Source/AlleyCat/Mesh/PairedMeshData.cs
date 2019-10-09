using AlleyCat.Mesh.Generic;
using EnsureThat;

namespace AlleyCat.Mesh
{
    public class PairedMeshData : ArrayMeshData<MorphableVertex>
    {
        public IMeshData Base { get; }

        public PairedMeshData(SimpleMeshData source, IMeshData basis) : base(source.Key, source.Source, source.FormatMask)
        {
            Ensure.That(source, nameof(source)).IsNotNull();
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Base = basis;
        }

        protected override MorphableVertex CreateVertex(int index) => new MorphableVertex(this, Base, index);
    }

    public static class PairedMeshDataExtensions
    {
        public static IMeshData<MorphableVertex> Join(this IMeshData basis, SimpleMeshData shape) =>
            new PairedMeshData(shape, basis);
    }
}
