using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using Godot.Collections;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Mesh
{
    public class MeshSurface : IMeshArray
    {
        public ArrayMesh Mesh { get; }

        public uint FormatMask { get; }

        public int Index { get; }

        public string Name
        {
            get => Mesh.SurfaceGetName(Index);
            set => Mesh.SurfaceSetName(Index, value);
        }

        public MeshData Base
        {
            get
            {
                if (_base.IsNone)
                {
                    _base = new MeshData(Mesh.SurfaceGetArrays(Index), FormatMask);
                }

                return _base.Head();
            }
        }

        public IEnumerable<MorphedMeshData> BlendShapes
        {
            get
            {
                if (_blendShapes != null) return _blendShapes;

                var mask = FormatMask;

                _blendShapes = Mesh.SurfaceGetBlendShapeArrays(Index)
                    .OfType<Array>()
                    .Map(source => new MorphedMeshData(source, Base, mask));

                return _blendShapes;
            }
        }

        public Godot.Mesh.PrimitiveType PrimitiveType => Mesh.SurfaceGetPrimitiveType(Index);

        private Option<MeshData> _base;

        private IEnumerable<MorphedMeshData> _blendShapes;

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
