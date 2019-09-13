using AlleyCat.Mesh.Generic;
using EnsureThat;

namespace AlleyCat.Mesh
{
    public class PairedMeshData : AbstractMeshData<MorphedVertex>
    {
        public IMeshData Base { get; }

        public PairedMeshData(ArrayMeshData source, IMeshData basis) : base(source.Key, source.Source, source.FormatMask)
        {
            Ensure.That(source, nameof(source)).IsNotNull();
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Base = basis;
        }

        protected override MorphedVertex CreateVertex(int index) => new MorphedVertex(this, Base, index);
    }

    public static class PairedMeshDataExtensions
    {
        public static IMeshData<MorphedVertex> Join(this IMeshData basis, ArrayMeshData shape) =>
            new PairedMeshData(shape, basis);
    }
}
