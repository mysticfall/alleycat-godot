namespace AlleyCat.Mesh
{
    public struct Vertex : IVertex
    {
        public IMeshData Source { get; }

        public int Index { get; }

        public Vertex(IMeshData source, int index)
        {
            Source = source;
            Index = index;
        }
    }
}
