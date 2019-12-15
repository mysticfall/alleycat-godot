using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using static Godot.Mesh;
using Array = Godot.Collections.Array;

namespace AlleyCat.Mesh
{
    public class ArrayMeshSurface : IMeshSurface
    {
        public string Key { get; }

        public ArrayMesh Mesh { get; }

        public uint FormatMask { get; }

        public int Index { get; }

        public IMeshData<Vertex> Data
        {
            get
            {
                if (_base.IsNone)
                {
                    _base = new SimpleMeshData(Key, Mesh.SurfaceGetArrays(Index), FormatMask);
                }

                return _base.Head();
            }
        }

        public PrimitiveType PrimitiveType { get; }

        public IEnumerable<IMeshData<MorphableVertex>> BlendShapes
        {
            get
            {
                if (_blendShapes != null) return _blendShapes;

                _blendShapes = Mesh.SurfaceGetBlendShapeArrays(Index)
                    .OfType<Array>()
                    .Map((i, source) => new BlendShapeData(Mesh.GetBlendShapeName(i), source, Data));

                return _blendShapes;
            }
        }

        public BlendShapeMode BlendShapeMode { get; }

        public Option<Material> Material => _material.Invoke();

        private Option<SimpleMeshData> _base;

        private IEnumerable<BlendShapeData> _blendShapes;

        private readonly Func<Option<Material>> _material;

        public ArrayMeshSurface(ArrayMesh mesh, int index)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();
            Ensure.That(index, nameof(index)).IsGte(0);

            Mesh = mesh;
            Index = index;

            Key = mesh.SurfaceGetName(index);
            FormatMask = Mesh.SurfaceGetFormat(index);
            PrimitiveType = Mesh.SurfaceGetPrimitiveType(Index);
            BlendShapeMode = Mesh.BlendShapeMode;

            _material = memo(() => Optional(Mesh.SurfaceGetMaterial(index)));
        }
    }
}
