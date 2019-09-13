using Godot.Collections;

namespace AlleyCat.Mesh
{
    public class ArrayMeshData : AbstractMeshData<Vertex>
    {
        public ArrayMeshData(string key, Array source, uint formatMask) : base(key, source, formatMask)
        {
        }

        protected override Vertex CreateVertex(int index) => new Vertex(this, index);
    }
}
