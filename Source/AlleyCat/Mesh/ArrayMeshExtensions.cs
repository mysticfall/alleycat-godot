using System.Collections.Generic;
using System.Linq;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using Godot.Collections;

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

        public static void CopyTo(this IMeshSurface surface, ArrayMesh target)
        {
            Ensure.That(surface, nameof(surface)).IsNotNull();
            Ensure.That(surface, nameof(surface)).IsNotNull();

            var index = target.SurfaceFindByName(surface.Key);

            if (index != -1)
            {
                target.SurfaceRemove(index);
            }

            var existingShapes = target.GetBlendShapeNames().ToHashSet();

            surface.BlendShapes.Map(b => b.Key).Filter(k => !existingShapes.Contains(k)).Iter(target.AddBlendShape);

            target.BlendShapeMode = surface.BlendShapeMode;

            var newIndex = target.GetSurfaceCount();

            Array CopyBlendShape(IMeshData<MorphedVertex> source)
            {
                var shape = new Array();

                source.Export()
                    .Cast<object>()
                    .Map((i, a) => i == (int) ArrayMesh.ArrayType.Index ? null : a)
                    .Iter(a => shape.Add(a));

                return shape;
            }

            var data = surface.Data.Export();
            var shapes = new Array(surface.BlendShapes.Map(CopyBlendShape));

            target.AddSurfaceFromArrays(surface.PrimitiveType, data, shapes);
            target.SurfaceSetName(newIndex, surface.Key);

            surface.Material.Iter(m => target.SurfaceSetMaterial(newIndex, m));
        }

        public static IEnumerable<string> GetBlendShapeNames(this ArrayMesh mesh)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();

            var count = mesh.GetBlendShapeCount();

            for (var i = 0; i < count; i++)
            {
                yield return mesh.GetBlendShapeName(i);
            }
        }
    }
}
