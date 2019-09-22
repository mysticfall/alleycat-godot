using System.Collections.Generic;
using EnsureThat;
using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class BlendShapeData : AbstractArrayMeshData<MorphedVertex>
    {
        public IMeshData Base { get; }

        public override IReadOnlyList<int> Indices => Base.Indices;

        public BlendShapeData(string key, Array source, IMeshData basis) : base(key, source, basis.FormatMask)
        {
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Base = basis;
        }

        protected override MorphedVertex CreateVertex(int index) => new MorphedVertex(this, Base, index);
    }
}
