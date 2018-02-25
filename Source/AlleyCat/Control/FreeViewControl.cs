using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Control
{
    public class FreeViewControl : AutowiredNode, IActivatable
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node("..")]
        public Spatial Target { get; private set; }

        [Node]
        public InputBindings Rotation { get; private set; }

        [Node]
        public InputBindings Movement { get; private set; }

        [PostConstruct]
        private void OnInitialize()
        {
            Input.SetMouseMode(Input.MouseMode.Captured);

            Rotation
                .AsVector2Input()
                .Where(_ => Active)
                .Subscribe(v =>
                {
                    Target.GlobalRotate(new Vector3(0, 1, 0), -v.x);
                    Target.RotateObjectLocal(new Vector3(1, 0, 0), -v.y);
                })
                .AddTo(this);

            Movement
                .AsVector2Input()
                .Where(_ => Active)
                .Select(v => new Vector3(v.x, 0, -v.y))
                .Select(v => v * 0.1f)
                .Subscribe(Target.TranslateObjectLocal)
                .AddTo(this);
        }
    }
}
