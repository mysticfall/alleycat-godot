using System;
using System.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class Blender2D : AnimationControl
    {
        public Vector2 Position
        {
            get => _position.Value;
            set => _position.OnNext(value);
        }

        protected string Parameter { get; }

        private readonly BehaviorSubject<Vector2> _position;

        public Blender2D(string parameter, AnimationGraphContext context) : base(context)
        {
            Ensure.That(parameter, nameof(parameter)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            Parameter = parameter;

            var current = (Vector2) context.AnimationTree.Get(parameter);

            _position = new BehaviorSubject<Vector2>(current).DisposeWith(this);

            _position
                .Subscribe(v => context.AnimationTree.Set(parameter, v))
                .DisposeWith(this);
        }

        public static Option<Blender2D> TryCreate(
            string name,
            IAnimationGraph parent,
            AnimationGraphContext context)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            if (parent.FindAnimationNode<AnimationNodeBlendSpace2D>(name).IsNone) return None;

            var parameter = string.Join("/",
                new[] {"parameters", parent.Path, name, "blend_position"}.Where(v => v.Length > 0));

            return new Blender2D(parameter, context);
        }
    }

    public static class Blender2DExtensions
    {
        public static Option<Blender2D> FindBlender2D(this IAnimationGraph graph, string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.FindDescendantControl<Blender2D>(path);
        }
    }
}
