using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Control;
using AlleyCat.Motion;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

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

        public override IEnumerable<Type> ProvidedTypes => Seq(typeof(IPerspectiveView), typeof(IOrbiter));
    }
}
