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

            using (var e = indexed.GetEnumerator())
            {
                while (true)
                {
                    var points = new TVertex[3];

                    if (!e.MoveNext()) break;

                    points[0] = e.Current;

                    if (!e.MoveNext()) break;
                    
                    points[1] = e.Current;

                    var hasMore = e.MoveNext();

                    points[2] = e.Current;

                    yield return new Triangle<TVertex>(points);

                    if (!hasMore) break;
                }
            }
        }
    }
}
