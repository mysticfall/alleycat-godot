using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public class GeometryDebugger : ImmediateGeometry
    {
        public Option<Material> Material { get; set; }

        public override void _EnterTree()
        {
            base._EnterTree();

            Material = new SpatialMaterial {VertexColorUseAsAlbedo = true, FlagsUnshaded = true};
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            Material.Iter(m => m.Free());
            Material = None;
        }

        public void DrawOrientation(Transform transform) => DrawOrientation(transform.origin, transform.basis);

        public void DrawOrientation(Vector3 origin, Basis basis) => DrawOrientation(origin, basis.Quat());

        public void DrawOrientation(Vector3 origin, Quat rotation) =>
            DrawOrientation(
                origin,
                rotation.Xform(Vector3.Up),
                rotation.Xform(Vector3.Forward),
                rotation.Xform(Vector3.Right));

        public void DrawOrientation(Vector3 origin, Vector3 up, Vector3 forward, Vector3 right)
        {
            DrawLine(origin, up, new Color(0, 1, 0));
            DrawLine(origin, forward, new Color(0, 0, 1));
            DrawLine(origin, right, new Color(1, 0, 0));
        }

        public void DrawLine(Vector3 origin, Vector3 direction, Color color)
        {
            Material.Iter(SetMaterialOverride);

            Begin(Mesh.PrimitiveType.Lines);

            SetColor(color);

            AddVertex(origin);
            AddVertex(origin + direction);

            End();
        }
    }
}
