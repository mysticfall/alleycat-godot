using AlleyCat.Common;
using EnsureThat;

namespace AlleyCat.Mesh
{
    public class BlendMapSet : IIdentifiable
    {
        public string Key { get; }

        public BlendMap Position { get; }

        public BlendMap Normal { get; }

        public BlendMapSet(string key, BlendMap position, BlendMap normal)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(position, nameof(position)).IsNotNull();
            Ensure.That(normal, nameof(normal)).IsNotNull();

            Key = key;
            Position = position;
            Normal = normal;
        }
    }
}
