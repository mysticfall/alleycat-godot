using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class SimpleMeshData : ArrayMeshData<Vertex>
    {
        public SimpleMeshData(string key, Array source, uint formatMask) : base(key, source, formatMask)
        {
        }

        protected override Vertex CreateVertex(int index) => new Vertex(this, index);
    }
}
