using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Mesh
{
    public class BlendMapSet : IIdentifiable
    {
        public string Key { get; }

        public BlendMap Position { get; }

        public BlendMap Normal { get; }

        public ISet<Vector3> Seams { get; }

        public BlendMapSet(string key, BlendMap position, BlendMap normal, ISet<Vector3> seams)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(position, nameof(position)).IsNotNull();
            Ensure.That(normal, nameof(normal)).IsNotNull();
            Ensure.That(seams, nameof(seams)).IsNotNull();

            Key = key;
            Position = position;
            Normal = normal;
            Seams = seams;
        }
    }
}
