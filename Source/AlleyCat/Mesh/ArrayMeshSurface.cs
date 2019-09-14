using System.Collections.Generic;
using System.Linq;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using Godot.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
using static Godot.Mesh;

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
                    _base = new ArrayMeshData(Key, Mesh.SurfaceGetArrays(Index), FormatMask);
                }

                return _base.Head();
            }
        }

        public PrimitiveType PrimitiveType { get; }

        public IEnumerable<IMeshData<MorphedVertex>> BlendShapes
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

        public Option<Material> Material { get; }

        private Option<ArrayMeshData> _base;

        private IEnumerable<BlendShapeData> _blendShapes;

        private Option<Material> _material;

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
            Material = Optional(_ => Mesh.SurfaceGetMaterial(index));
        }
    }
}
