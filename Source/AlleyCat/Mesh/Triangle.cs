using System.Collections.Generic;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Mesh
{
    public struct Triangle<TVertex> : IPolygon<TVertex> where TVertex : IVertex
    {
        public TVertex Point1 => Points[0];

        public TVertex Point2 => Points[1];

        public TVertex Point3 => Points[2];

        public Arr<TVertex> Points { get; }

        public Triangle(Arr<TVertex> points)
        {
            Ensure.That(points.Count, nameof(points)).Is(3);

            Points = points;
        }
    }

    public static class TriangleExtensions
    {
        public static IEnumerable<Triangle<TVertex>> Triangles<TVertex>(this IMeshData<TVertex> data)
            where TVertex : IVertex
        {
            var indexed = data.Indexed;
            var e = indexed.GetEnumerator();
            var index = 0;
            var points = new TVertex[3];

            while (e.MoveNext())
            {
                points[index++] = e.Current;

                if (index == 3)
                {
                    index = 0;
                    yield return new Triangle<TVertex>(points);
                }
            }

            e.Dispose();
        }
    }
}
