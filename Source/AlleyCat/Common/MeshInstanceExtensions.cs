using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public static class MeshInstanceExtensions
    {
        public static Option<Material> FindSurfaceMaterial(this MeshInstance mesh, int index)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();
            Ensure.That(index, nameof(index)).IsGte(0);

            return Optional(mesh.GetSurfaceMaterial(index)) | Optional(mesh.Mesh.SurfaceGetMaterial(index));
        }
    }
}
