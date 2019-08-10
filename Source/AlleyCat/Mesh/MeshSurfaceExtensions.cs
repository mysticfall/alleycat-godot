using System.Collections.Generic;
using EnsureThat;
using Godot;

namespace AlleyCat.Mesh
{
    public static class MeshSurfaceExtensions
    {
        public static MeshSurface GetSurface(this ArrayMesh mesh, int index) => new MeshSurface(mesh, index);

        public static IEnumerable<MeshSurface> GetSurfaces(this ArrayMesh mesh)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();

            var count = mesh.GetSurfaceCount();

            for (var i = 0; i < count; i++)
            {
                yield return new MeshSurface(mesh, i);
            }
        }
    }
}
