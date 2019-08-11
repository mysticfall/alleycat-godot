using System.Linq;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Mesh
{
    public interface IPolygon<TVertex> where TVertex : IVertex
    {
        Arr<TVertex> Points { get; }
    }

    public static class PolygonExtensions
    {
        public static Vector3 Center<TVertex>(this IPolygon<TVertex> polygon) where TVertex : IVertex
        {
            Ensure.That(polygon, nameof(polygon)).IsNotNull();

            var points = polygon.Points;

            return points.Map(p => p.Position()).Aggregate((p1, p2) => p1 + p2) / points.Count;
        }
    }
}
