using EnsureThat;
using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class MorphedMeshData : AbstractMeshData<MorphedVertex>
    {
        public IMeshData Base { get; }

        public MorphedMeshData(Array source, IMeshData basis, uint formatMask) : base(source, formatMask)
        {
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Base = basis;
        }

        protected override MorphedVertex CreateVertex(int index) => new MorphedVertex(this, Base, index);
    }
}
