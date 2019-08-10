using Godot;
using LanguageExt;
using static Godot.ArrayMesh;
using static LanguageExt.Prelude;

namespace AlleyCat.Mesh
{
    public struct VertexData
    {
        public Vector3 Position => _source.Vertices[_index];

        public Vector3 Normal => _source.Normals[_index];

        public Option<float[]> Tangents => Supports(ArrayFormat.Tangent) ? Some(_source.Tangents[_index]) : None;

        public Option<Color> Color => Supports(ArrayFormat.Color) ? Some(_source.Colors[_index]) : None;

        public Option<int[]> Bones => Supports(ArrayFormat.Bones) ? Some(_source.Bones[_index]) : None;

        public Option<float[]> Weights => Supports(ArrayFormat.Weights) ? Some(_source.Weights[_index]) : None;

        public Option<Vector2> UV => Supports(ArrayFormat.TexUv) ? Some(_source.UV[_index]) : None;

        public Option<Vector2> UV2 => Supports(ArrayFormat.TexUv2) ? Some(_source.UV2[_index]) : None;

        private readonly MeshSurfaceData _source;

        private readonly int _index;

        public VertexData(MeshSurfaceData source, int index)
        {
            _source = source;
            _index = index;
        }

        private bool Supports(ArrayFormat format) => _source.SupportsFormat(format);
    }
}
