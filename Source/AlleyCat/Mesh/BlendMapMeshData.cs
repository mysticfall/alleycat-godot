using EnsureThat;
using Godot;

namespace AlleyCat.Mesh
{
    public class BlendMapMeshData : MorphableMeshData
    {
        public BlendMapSet BlendMap { get; }

        public BlendMapMeshData(BlendMapSet blendMap, IMeshData basis) : this(blendMap.Key, blendMap, basis)
        {
        }

        public BlendMapMeshData(string key, BlendMapSet blendMap, IMeshData basis) : base(key, basis)
        {
            Ensure.That(blendMap, nameof(blendMap)).IsNotNull();

            BlendMap = blendMap;
        }

        protected override Vector3 ReadVertex(int index) =>
            Base.Vertices[index] + BlendMap.Position.GetOffset(UV[index]);

        protected override Vector3 ReadNormal(int index) =>
            Base.Normals[index] + BlendMap.Normal.GetOffset(UV[index]);
    }
}
