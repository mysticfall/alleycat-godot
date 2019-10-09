using System;
using System.Collections;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Mesh
{
    public abstract class MorphableMeshData : AbstractMeshData<MorphableVertex>
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

        protected MorphableMeshData(string key, IMeshData basis) : base(key)
        {
            Ensure.That(basis, nameof(basis)).IsNotNull();

            Base = basis;

            var count = Base.Vertices.Count;

            _vertices = new VertexList(ReadVertex, count);
            _normals = new VertexList(ReadNormal, count);
        }

        protected override MorphableVertex CreateVertex(int index) => new MorphableVertex(this, Base, index);

        protected abstract Vector3 ReadVertex(int index);

        protected abstract Vector3 ReadNormal(int index);

        internal class VertexList : IReadOnlyList<Vector3>
        {
            public int Count { get; }

            private readonly Func<int, Vector3> _reader;

            private readonly Option<Vector3>[] _data;

            public VertexList(Func<int, Vector3> reader, int count)
            {
                Count = count;

                _reader = reader;
                _data = new Option<Vector3>[count];
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
                var value = _reader(index);

                _data[index] = Some(value);

                return value;
            }

            public Vector3 this[int index] => _data[index].Match(identity, () => Read(index));
        }
    }
}
