namespace AlleyCat.Mesh
{
    public struct MorphedVertex : IVertex
    {
        public IMeshData Source { get; }

        public IVertex Basis { get; }

        public int Index { get; }

        public MorphedVertex(IMeshData source, IMeshData basis, int index)
        {
            Source = source;
            Basis = new Vertex(basis, index);
            Index = index;
        }
    }
}
