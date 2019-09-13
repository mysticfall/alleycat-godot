using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class MeshData : AbstractMeshData<Vertex>
    {
        public MeshData(string key, Array source, uint formatMask) : base(key, source, formatMask)
        {
        }

        protected override Vertex CreateVertex(int index) => new Vertex(this, index);
    }
}
