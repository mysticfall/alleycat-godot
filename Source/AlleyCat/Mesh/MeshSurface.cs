using System.Collections.Generic;
using AlleyCat.Mesh.Generic;
using Godot;
using LanguageExt;
using static Godot.Mesh;

namespace AlleyCat.Mesh
{
    public class MeshSurface : IMeshSurface
    {
        public string Key { get; }

        public IMeshData<Vertex> Data { get; }

        public uint FormatMask => Data.FormatMask;

        public PrimitiveType PrimitiveType { get; }

        public IEnumerable<IMeshData<MorphableVertex>> BlendShapes { get; }

        public BlendShapeMode BlendShapeMode { get; }

        public Option<Material> Material { get; }

        public MeshSurface(
            string key,
            PrimitiveType primitiveType,
            IMeshData<Vertex> data,
            IEnumerable<IMeshData<MorphableVertex>> blendShapes,
            BlendShapeMode blendShapeMode,
            Option<Material> material)
        {
            Key = key;
            PrimitiveType = primitiveType;
            Data = data;
            BlendShapes = blendShapes;
            BlendShapeMode = blendShapeMode;
            Material = material;
        }
    }
}
