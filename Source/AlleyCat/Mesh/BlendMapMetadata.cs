using Godot;
using Newtonsoft.Json;

namespace AlleyCat.Mesh
{
    public class BlendMapMetadata
    {
        public BlendMapTextureMetadata Position { get; }

        public BlendMapTextureMetadata Normal { get; }

        [JsonConstructor]
        public BlendMapMetadata(BlendMapTextureMetadata position, BlendMapTextureMetadata normal)
        {
            Position = position;
            Normal = normal;
        }
    }

    public class BlendMapTextureMetadata
    {
        public string Texture { get; }

        public Vector3 Min { get; }

        public Vector3 Max { get; }

        [JsonConstructor]
        public BlendMapTextureMetadata(string texture, Vector3 min, Vector3 max)
        {
            Texture = texture;
            Min = min;
            Max = max;
        }
    }
}
