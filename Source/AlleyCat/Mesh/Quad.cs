using System.Collections.Generic;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Mesh
{
    public struct Quad<TVertex> : IFace<TVertex> where TVertex : IVertex
    {
        public TVertex Point1 => Points[0];

        public TVertex Point2 => Points[1];

        public TVertex Point3 => Points[2];
        
        public TVertex Point4 => Points[3];

        public Arr<TVertex> Points { get; }

        public Quad(Arr<TVertex> points)
        {
            Ensure.That(points.Count, nameof(points)).Is(4);

            Points = points;
        }
    }

    public static class QuadExtensions
    {
        public static IEnumerable<Quad<TVertex>> Quads<TVertex>(this IMeshData<TVertex> data)
            where TVertex : IVertex
        {
            var indexed = data.Indexed;
            var e = indexed.GetEnumerator();
            var index = 0;
            var points = new TVertex[4];

            while (e.MoveNext())
            {
                points[index++] = e.Current;

                if (index == 4)
                {
                    index = 0;
                    yield return new Quad<TVertex>(points);
                }
            }

            e.Dispose();
        }
    }
}
