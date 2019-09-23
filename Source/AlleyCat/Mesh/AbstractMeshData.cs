using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using Godot.Collections;

namespace AlleyCat.Mesh
{
    public abstract class AbstractMeshData<TVertex> : IMeshData<TVertex> where TVertex : IVertex
    {
        public string Key { get; }

        public abstract IReadOnlyList<Vector3> Vertices { get; }

        public abstract IReadOnlyList<Vector3> Normals { get; }

        public abstract IReadOnlyList<float> Tangents { get; }

        public abstract IReadOnlyList<Color> Colors { get; }

        public abstract IReadOnlyList<int> Bones { get; }

        public abstract IReadOnlyList<float> Weights { get; }

        public abstract IReadOnlyList<Vector2> UV { get; }

        public abstract IReadOnlyList<Vector2> UV2 { get; }

        public abstract IReadOnlyList<int> Indices { get; }

        public virtual int Count => Vertices.Count;

        public abstract uint FormatMask { get; }

        protected AbstractMeshData(string key)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            Key = key;
        }

        public IEnumerator<TVertex> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return CreateVertex(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<TVertex> Indexed => Indices.Select(CreateVertex);

        public TVertex this[int index] => CreateVertex(Indices[index]);

        protected abstract TVertex CreateVertex(int index);

        public abstract Array Export();
    }
}
