using System;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static Godot.ArrayMesh;
using Array = Godot.Collections.Array;

namespace AlleyCat.Mesh
{
    public abstract class ArrayMeshData<TVertex> : AbstractMeshData<TVertex> where TVertex : IVertex
    {
        public override IReadOnlyList<Vector3> Vertices => _vertices ?? (_vertices = Read<Vector3>(ArrayType.Vertex));

        public override IReadOnlyList<Vector3> Normals => _normals ?? (_normals = Read<Vector3>(ArrayType.Normal));

        public override IReadOnlyList<float> Tangents => _tangents ?? (_tangents = Read<float>(ArrayType.Tangent));

        public override IReadOnlyList<Color> Colors => _colors ?? (_colors = Read<Color>(ArrayType.Color));

        public override IReadOnlyList<int> Bones => _bones ?? (_bones = Read<int>(ArrayType.Bones));

        public override IReadOnlyList<float> Weights => _weights ?? (_weights = Read<float>(ArrayType.Weights));

        public override IReadOnlyList<Vector2> UV => _uv ?? (_uv = Read<Vector2>(ArrayType.TexUv));

        public override IReadOnlyList<Vector2> UV2 => _uv2 ?? (_uv2 = Read<Vector2>(ArrayType.TexUv2));

        public override IReadOnlyList<int> Indices => _indices ?? (_indices = Read<int>(ArrayType.Index));

        public override uint FormatMask { get; }

        public Array Source { get; }

        private readonly int _count;

        private Map<ArrayType, object> _cache;

        private Vector3[] _vertices;

        private Vector3[] _normals;

        private float[] _tangents;

        private Color[] _colors;

        private int[] _bones;

        private float[] _weights;

        private Vector2[] _uv;

        private Vector2[] _uv2;

        private int[] _indices;

        protected ArrayMeshData(string key, Array source, uint formatMask) : base(key)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;

            FormatMask = formatMask;

            _vertices = null;
            _normals = null;
            _tangents = null;
            _colors = null;
            _bones = null;
            _weights = null;
            _uv = null;
            _uv2 = null;
            _indices = null;
        }

        public override Array Export() => Source;

        private T[] Read<T>(ArrayType tpe)
        {
            ArrayFormat format;

            switch (tpe)
            {
                case ArrayType.Vertex:
                    format = ArrayFormat.Vertex;
                    break;
                case ArrayType.Normal:
                    format = ArrayFormat.Normal;
                    break;
                case ArrayType.Tangent:
                    format = ArrayFormat.Tangent;
                    break;
                case ArrayType.Color:
                    format = ArrayFormat.Color;
                    break;
                case ArrayType.TexUv:
                    format = ArrayFormat.TexUv;
                    break;
                case ArrayType.TexUv2:
                    format = ArrayFormat.TexUv2;
                    break;
                case ArrayType.Bones:
                    format = ArrayFormat.Bones;
                    break;
                case ArrayType.Weights:
                    format = ArrayFormat.Weights;
                    break;
                case ArrayType.Index:
                    format = ArrayFormat.Index;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tpe), tpe, "Unknown array type: " + tpe);
            }

            if (!this.SupportsFormat(format))
            {
                throw new InvalidOperationException($"The mesh does not contain the data type: '{tpe}'.");
            }

            return (T[]) Source[(int) tpe];
        }
    }
}
