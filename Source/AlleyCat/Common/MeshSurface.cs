using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using Godot.Collections;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public struct MeshSurface : IMeshData
    {
        public ArrayMesh Mesh { get; }

        public uint FormatMask { get; }

        public int Index { get; }

        public string Name
        {
            get => Mesh.SurfaceGetName(Index);
            set => Mesh.SurfaceSetName(Index, value);
        }

        public MeshSurfaceData Base
        {
            get
            {
                if (_base.IsNone)
                {
                    _base = new MeshSurfaceData(Mesh.SurfaceGetArrays(Index), FormatMask);
                }

                return _base.Head();
            }
        }

        public IEnumerable<MeshSurfaceData> BlendShapes
        {
            get
            {
                if (_blendShapes != null) return _blendShapes;

                var mask = FormatMask;

                _blendShapes = Mesh.SurfaceGetBlendShapeArrays(Index)
                    .OfType<Array>()
                    .Map(a => new MeshSurfaceData(a, mask));

                return _blendShapes;
            }
        }

        public Mesh.PrimitiveType PrimitiveType => Mesh.SurfaceGetPrimitiveType(Index);

        private Option<MeshSurfaceData> _base;

        private IEnumerable<MeshSurfaceData> _blendShapes;

        public MeshSurface(ArrayMesh mesh, int index)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();
            Ensure.That(index, nameof(index)).IsGte(0);

            Mesh = mesh;
            Index = index;

            FormatMask = Mesh.SurfaceGetFormat(index);

            _base = None;
            _blendShapes = null;
        }
    }
}
