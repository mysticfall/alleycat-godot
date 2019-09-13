using System.Collections;
using System.Collections.Generic;
using AlleyCat.Common;
using Godot;
using LanguageExt;

namespace AlleyCat.Mesh
{
    public interface IMeshData : IMeshArray, IEnumerable, IIdentifiable
    {
        Arr<Vector3> Vertices { get; }

        Arr<Vector3> Normals { get; }

        Arr<float[]> Tangents { get; }

        Arr<Color> Colors { get; }

        Arr<int[]> Bones { get; }

        Arr<float[]> Weights { get; }

        Arr<Vector2> UV { get; }

        Arr<Vector2> UV2 { get; }

        Arr<int> Indices { get; }
    }

    namespace Generic
    {
        public interface IMeshData<out TVertex> : IMeshData, IReadOnlyList<TVertex>
        {
            IEnumerable<TVertex> Indexed { get; }
        }
    }
}
