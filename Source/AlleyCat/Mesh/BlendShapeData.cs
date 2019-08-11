using AlleyCat.Common;
using EnsureThat;
using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class BlendShapeData : AbstractMeshData<MorphedVertex>, IIdentifiable
    {
        public string Key { get; }

        public IMeshData Base { get; }

        public BlendShapeData(string key, Array source, IMeshData basis, uint formatMask) : base(source, formatMask)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Key = key;
            Base = basis;
        }

        protected override MorphedVertex CreateVertex(int index) => new MorphedVertex(this, Base, index);
    }
}
