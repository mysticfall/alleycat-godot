using EnsureThat;
using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class BlendShapeData : AbstractMeshData<MorphedVertex>
    {
        public IMeshData Base { get; }

        public BlendShapeData(
            string key, Array source, IMeshData basis, uint formatMask) : base(key, source, formatMask)
        {
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Base = basis;
        }

        protected override MorphedVertex CreateVertex(int index) => new MorphedVertex(this, Base, index);
    }
}
