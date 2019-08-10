using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Mesh
{
    public static class MeshInstanceExtensions
    {
        public static Option<Material> FindSurfaceMaterial(this MeshInstance mesh, int index)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();

            return Optional(mesh.GetSurfaceMaterial(index)) | Optional(mesh.Mesh.SurfaceGetMaterial(index));
        }
    }
}
