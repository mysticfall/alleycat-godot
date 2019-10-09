using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;

namespace AlleyCat.Mesh
{
    public class PairedMeshData : MorphableMeshData
    {
        public IMeshData Data { get; }

        public PairedMeshData(string key, IMeshData data, IMeshData basis) : base(key, basis)
        {
            Ensure.That(data, nameof(data)).IsNotNull();

            Data = data;
        }

        protected override Vector3 ReadVertex(int index) => Data.Vertices[index];

        protected override Vector3 ReadNormal(int index) => Data.Normals[index];
    }

    public static class PairedMeshDataExtensions
    {
        public static IMeshData<MorphableVertex> Join(this IMeshData basis, string key, IMeshData shape) =>
            new PairedMeshData(key, shape, basis);
    }
}
