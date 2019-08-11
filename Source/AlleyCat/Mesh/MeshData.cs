using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class MeshData : AbstractMeshData<Vertex>
    {
        public MeshData(Array source, uint formatMask) : base(source, formatMask)
        {
        }

        protected override Vertex CreateVertex(int index) => new Vertex(this, index);
    }
}
