using System.Linq;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Mesh
{
    public interface IFace<TVertex> where TVertex : IVertex
    {
        Arr<TVertex> Points { get; }
    }

    public static class PolygonExtensions
    {
        public static Vector3 Center<TVertex>(this IFace<TVertex> face) where TVertex : IVertex
        {
            Ensure.That(face, nameof(face)).IsNotNull();

            var points = face.Points;

            return points.Map(p => p.Position()).Aggregate((p1, p2) => p1 + p2) / points.Count;
        }
    }
}
