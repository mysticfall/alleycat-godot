using System.Collections;
using System.Collections.Generic;
using AlleyCat.Common;
using Godot;
using Godot.Collections;
using LanguageExt;

namespace AlleyCat.Mesh
{
    public interface IMeshData : IMeshArray, IEnumerable, IIdentifiable
    {
        IReadOnlyList<Vector3> Vertices { get; }

        IReadOnlyList<Vector3> Normals { get; }

        IReadOnlyList<float[]> Tangents { get; }

        IReadOnlyList<Color> Colors { get; }

        IReadOnlyList<int[]> Bones { get; }

        IReadOnlyList<float[]> Weights { get; }

        IReadOnlyList<Vector2> UV { get; }

        IReadOnlyList<Vector2> UV2 { get; }

        IReadOnlyList<int> Indices { get; }

        Array Export();
    }

    namespace Generic
    {
        public interface IMeshData<out TVertex> : IMeshData, IReadOnlyList<TVertex>
        {
            IEnumerable<TVertex> Indexed { get; }
        }
    }
}
