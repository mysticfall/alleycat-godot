using System.Collections.Generic;
using EnsureThat;
using Godot;

namespace AlleyCat.Mesh
{
    public static class ArrayMeshExtensions
    {
        public static IMeshSurface GetSurface(this ArrayMesh mesh, int index) => new ArrayMeshSurface(mesh, index);

        public static IEnumerable<IMeshSurface> GetSurfaces(this ArrayMesh mesh)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();

            var count = mesh.GetSurfaceCount();

            for (var i = 0; i < count; i++)
            {
                yield return new ArrayMeshSurface(mesh, i);
            }
        }
    }
}
