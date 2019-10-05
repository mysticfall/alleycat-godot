using System.Collections;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Mesh
{
    public class BlendMapMeshData : AbstractMeshData<MorphedVertex>
    {
        public IMeshData Base { get; }

        public override uint FormatMask => Base.FormatMask;

        public override IReadOnlyList<Vector3> Vertices => _vertices;

        public override IReadOnlyList<Vector3> Normals => _normals;

        public override IReadOnlyList<float> Tangents => Base.Tangents;

        public override IReadOnlyList<Color> Colors => Base.Colors;

        public override IReadOnlyList<int> Bones => Base.Bones;

        public override IReadOnlyList<float> Weights => Base.Weights;

        public override IReadOnlyList<Vector2> UV => Base.UV;

        public override IReadOnlyList<Vector2> UV2 => Base.UV2;

        public override IReadOnlyList<int> Indices => Base.Indices;

        private readonly VertexList _vertices;

        private readonly VertexList _normals;

        public BlendMapMeshData(BlendMapSet blendMap, IMeshData basis) : base(blendMap.Key)
        {
            Ensure.That(blendMap, nameof(blendMap)).IsNotNull();
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Base = basis;

            _vertices = new VertexList(blendMap.Position, basis.Vertices, basis.UV);
            _normals = new VertexList(blendMap.Normal, basis.Normals, basis.UV);
        }

        protected override MorphedVertex CreateVertex(int index) => new MorphedVertex(this, Base, index);

        internal class VertexList : IReadOnlyList<Vector3>
        {
            public BlendMap BlendMap { get; }

            public IReadOnlyList<Vector3> Basis { get; }

            public IReadOnlyList<Vector2> UV { get; }

            public Option<Vector3>[] Data { get; }

            public int Count => Data.Length;

            public VertexList(BlendMap blendMap, IReadOnlyList<Vector3> basis, IReadOnlyList<Vector2> uv)
            {
                BlendMap = blendMap;
                Basis = basis;
                UV = uv;

                Data = new Option<Vector3>[basis.Count];
            }

            public IEnumerator<Vector3> GetEnumerator()
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private Vector3 Read(int index)
            {
                var offset = BlendMap.GetOffset(UV[index]);
                var value = Basis[index] + offset;

                Data[index] = Some(value);

                return value;
            }

            public Vector3 this[int index] => Data[index].Match(identity, () => Read(index));
        }
    }
}
