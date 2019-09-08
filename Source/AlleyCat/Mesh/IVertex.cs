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

        public static Option<float[]> Tangents(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Tangent)
                ? Some(vertex.Source.Tangents[vertex.Index])
                : None;
        }

        public static Option<Color> Color(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Color)
                ? Some(vertex.Source.Colors[vertex.Index])
                : None;
        }

        public static Option<int[]> Bones(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Bones)
                ? Some(vertex.Source.Bones[vertex.Index])
                : None;
        }

        public static Option<float[]> Weights(this IVertex vertex)
        {
            return vertex.Source.SupportsFormat(ArrayFormat.Weights)
                ? Some(vertex.Source.Weights[vertex.Index])
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
    }
}
