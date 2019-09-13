using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Mesh.Generic;
using static Godot.Mesh;

namespace AlleyCat.Mesh
{
    public interface IMeshSurface : IMeshArray, IIdentifiable
    {
        IMeshData<Vertex> Base { get; }

        IEnumerable<IMeshData<MorphedVertex>> BlendShapes { get; }

        PrimitiveType PrimitiveType { get; }
    }
}
