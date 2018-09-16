using System;
using System.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class Blender2D : AnimationControl
    {
        public Vector2 Position
        {
            get => _position.Value;
            set => _position.Value = value;
        }

        protected string Parameter { get; }

        private readonly ReactiveProperty<Vector2> _position;

        public Blender2D([NotNull] string parameter, AnimationGraphContext context) : base(context)
        {
            Ensure.Any.IsNotNull(parameter, nameof(parameter));

            Parameter = parameter;

            var current = (Vector2) context.AnimationTree.Get(parameter);

            _position = new ReactiveProperty<Vector2>(current);
            _position.Subscribe(v => context.AnimationTree.Set(parameter, v));
        }

        public override void Dispose()
        {
            _position?.Dispose();
        }

        public static Blender2D Create(
            [NotNull] string name,
            [NotNull] IAnimationGraph parent,
            [NotNull] AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));
            Ensure.Any.IsNotNull(context, nameof(context));

            if (!(parent.GetAnimationNode(name) is AnimationNodeBlendSpace2D)) return null;

            var parameter = string.Join("/",
                new[] {"parameters", parent.Path, name, "blend_position"}.Where(v => v.Length > 0));

            return new Blender2D(parameter, context);
        }
    }

    public static class Blender2DExtensions
    {
        [CanBeNull]
        public static Blender2D GetBlender2D(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.GetDescendantControl(path) as Blender2D;
        }
    }
}
