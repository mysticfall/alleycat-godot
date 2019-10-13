using System;
using System.Linq;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using Supercluster.KDTree;

namespace AlleyCat.Mesh
{
    public class AdaptiveMeshData : MorphableMeshData
    {
        public IMeshData<MorphableVertex> Source { get; }

        public int Samples { get; }

        protected KDTree<float, MorphableVertex> Tree { get; }

        public AdaptiveMeshData(
            string key, 
            IMeshData<MorphableVertex> source, 
            IMeshData basis,
            int samples = 4) : base(key, basis)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
            Samples = Mathf.Max(samples, 2);

            var nodes = source.ToArray();
            var points = nodes.Map(n => n.Position()).Map(ToArray).ToArray();

            double GetDistance(float[] v1, float[] v2) => ToVector3(v1).DistanceTo(ToVector3(v2));

            Tree = new KDTree<float, MorphableVertex>(3, points, nodes, GetDistance);
        }

        protected override Vector3 ReadVertex(int index) => Base.Vertices[index] + ReadOffset(index, v => v.Position());

        protected override Vector3 ReadNormal(int index) => Base.Normals[index] + ReadOffset(index, v => v.Normal());

        private Vector3 ReadOffset(int index, Func<IVertex, Vector3> extractor)
        {
            var pos = Base.Vertices[index];

            var hits = Tree.NearestNeighbors(ToArray(pos), Samples)
                .Map(v => v.Item2)
                .Map(h => (h.Basis.Position().DistanceSquaredTo(pos), extractor(h) - extractor(h.Basis)))
                .ToArr();

            var total = hits.Map(h => h.Item1).Sum();

            return hits.Map(h => h.Item2 * h.Item1 / total).Aggregate((v1, v2) => v1 + v2);
        }

        private static float[] ToArray(Vector3 pos) => new[] {pos.x, pos.y, pos.z};

        private static Vector3 ToVector3(float[] pos) => new Vector3(pos[0], pos[1], pos[2]);
    }
}
