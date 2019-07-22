using System;
using System.Collections.Generic;
using System.Threading;
using EnsureThat;
using Godot;
using LanguageExt;
using static Godot.ArrayMesh;
using Array = Godot.Collections.Array;

namespace AlleyCat.Common
{
    public struct MeshSurfaceData : IMeshData
    {
        public IList<Vector3> Vertices => _vertices ?? (_vertices = Read<Vector3>(ArrayType.Vertex));

        public IList<Vector3> Normals => _normals ?? (_normals = Read<Vector3>(ArrayType.Normal));

        public IList<float[]> Tangents => _tangents ?? (_tangents = Read<float[]>(ArrayType.Tangent));

        public IList<Color> Colors => _colors ?? (_colors = Read<Color>(ArrayType.Color));

        public IList<int[]> Bones => _bones ?? (_bones = Read<int[]>(ArrayType.Bones));

        public IList<float[]> Weights => _weights ?? (_weights = Read<float[]>(ArrayType.Weights));

        public IList<Vector2> UV => _uv ?? (_uv = Read<Vector2>(ArrayType.TexUv));

        public IList<Vector2> UV2 => _uv2 ?? (_uv2 = Read<Vector2>(ArrayType.TexUv2));

        public IList<int> Indices => _indices ?? (_indices = Read<int>(ArrayType.Index));

        public uint FormatMask { get; }

        private readonly Array _source;

        private Map<ArrayType, object> _cache;

        private IList<Vector3> _vertices;

        private IList<Vector3> _normals;

        private IList<float[]> _tangents;

        private IList<Color> _colors;

        private IList<int[]> _bones;

        private IList<float[]> _weights;

        private IList<Vector2> _uv;

        private IList<Vector2> _uv2;

        private IList<int> _indices;

        public MeshSurfaceData(Array source, uint formatMask)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            FormatMask = formatMask;

            _source = source;

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

        private IList<T> Read<T>(ArrayType tpe)
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
                throw new ThreadStateException($"The mesh does not contain the data type: '{tpe}'.");
            }

            return (T[]) _source[(int) tpe];
        }
    }
}
