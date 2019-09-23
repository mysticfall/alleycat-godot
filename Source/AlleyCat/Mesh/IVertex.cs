using System.Collections;
using System.Collections.Generic;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using static Godot.ArrayMesh;

namespace AlleyCat.Mesh
{
    public interface IVertex
    {
        IMeshData Source { get; }

        int Index { get; }
    }

    public static class VertexExtensions
    {
        public static Vector3 Position(this IVertex vertex) => vertex.Source.Vertices[vertex.Index];

        public static Vector3 Normal(this IVertex vertex) => vertex.Source.Normals[vertex.Index];

        public static Option<IReadOnlyList<float>> Tangents(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Tangent)
                ? Some<IReadOnlyList<float>>(new OffsetList<float>(vertex.Source.Tangents, vertex.Index, 4))
                : None;
        }

        public static Option<Color> Color(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Color)
                ? Some(vertex.Source.Colors[vertex.Index])
                : None;
        }

        public static Option<IReadOnlyList<int>> Bones(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Bones)
                ? Some<IReadOnlyList<int>>(new OffsetList<int>(vertex.Source.Bones, vertex.Index, 4))
                : None;
        }

        public static Option<IReadOnlyList<float>> Weights(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Weights)
                ? Some<IReadOnlyList<float>>(new OffsetList<float>(vertex.Source.Weights, vertex.Index, 4))
                : None;
        }

        public static Option<Vector2> UV(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.TexUv)
                ? Some(vertex.Source.UV[vertex.Index])
                : None;
        }

        public static Option<Vector2> UV2(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.TexUv2)
                ? Some(vertex.Source.UV2[vertex.Index])
                : None;
        }

        internal struct OffsetList<T> : IReadOnlyList<T>
        {
            public IReadOnlyList<T> Source { get; }

            public int Offset { get; }

            public int Count { get; }

            public OffsetList(IReadOnlyList<T> source, int offset, int count)
            {
                Source = source;
                Offset = offset;
                Count = count;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public T this[int index] => Source[Offset * Count + index];
        }
    }
}
