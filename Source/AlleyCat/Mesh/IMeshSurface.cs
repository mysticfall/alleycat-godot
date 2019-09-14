using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Mesh.Generic;
using Godot;
using LanguageExt;
using static Godot.Mesh;

namespace AlleyCat.Mesh
{
    public interface IMeshSurface : IMeshArray, IIdentifiable
    {
        IMeshData<Vertex> Data { get; }

        PrimitiveType PrimitiveType { get; }

        IEnumerable<IMeshData<MorphedVertex>> BlendShapes { get; }

        BlendShapeMode BlendShapeMode { get; }

        Option<Material> Material { get; }
    }
}
