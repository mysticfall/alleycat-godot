using System.Collections.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Mesh
{
    public class BlendMapMeshData : MorphableMeshData
    {
        public BlendMapSet BlendMap { get; }

        // Cache is needed because an ArrayMesh often contains duplicated vertices with different 
        // UV coordinates which can result in tearing at the texture seams. 
        private readonly IDictionary<Vector3, Vector3> _cache = new Dictionary<Vector3, Vector3>();

        public BlendMapMeshData(BlendMapSet blendMap, IMeshData basis) : this(blendMap.Key, blendMap, basis)
        {
        }

        public BlendMapMeshData(string key, BlendMapSet blendMap, IMeshData basis) : base(key, basis)
        {
            Ensure.That(blendMap, nameof(blendMap)).IsNotNull();

            BlendMap = blendMap;
        }

        protected override Vector3 ReadVertex(int index)
        {
            var basis = Base.Vertices[index];

            if (!BlendMap.Seams.Contains(basis))
            {
                return basis + BlendMap.Position.GetOffset(UV[index]);
            }

            return _cache.TryGetValue(basis).Match(identity, () =>
            {
                var value = basis + BlendMap.Position.GetOffset(UV[index]);

                _cache.Add(basis, value);

                return value;
            });
        }

        protected override Vector3 ReadNormal(int index) =>
            Base.Normals[index] + BlendMap.Normal.GetOffset(UV[index]);
    }
}
