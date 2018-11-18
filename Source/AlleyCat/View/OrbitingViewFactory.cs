using AlleyCat.Autowire;
using AlleyCat.Control;
using AlleyCat.Motion;
using Godot;
using LanguageExt;

namespace AlleyCat.View
{
    public abstract class OrbitingViewFactory<T> : OrbiterFactory<T> where T : OrbitingView
    {
        [Node(false)]
        public Option<Camera> Camera { get; set; }

        [Export] private NodePath _camera;

        [Node("Rotation", false)]
        public Option<IInputBindings> RotationInput { get; set; }

        [Node("Zoom", false)]
        public Option<IInputBindings> ZoomInput { get; set; }
    }
}
