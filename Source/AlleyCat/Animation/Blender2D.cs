using System;
using System.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
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

        public Blender2D(string key, string parameter, AnimationGraphContext context) : base(key, context)
        {
            Ensure.That(parameter, nameof(parameter)).IsNotNull();

            Parameter = parameter;

            var current = (Vector2) context.AnimationTree.Get(parameter);

            _position = new BehaviorSubject<Vector2>(current).DisposeWith(this);

            _position
                .Subscribe(v => context.AnimationTree.Set(parameter, v))
                .DisposeWith(this);

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                _position
                    .Subscribe(v => this.LogTrace("Changed blending position: {}.", v))
                    .DisposeWith(this);
            }
        }

        public static Option<Blender2D> TryCreate(
            string name,
            IAnimationGraph parent,
            AnimationGraphContext context)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();

            if (parent.FindAnimationNode<AnimationNodeBlendSpace2D>(name).IsNone) return None;

            var parameter = string.Join("/",
                new[] {"parameters", parent.Key, name, "blend_position"}.Where(v => v.Length > 0));

            return new Blender2D(string.Join(":", parent.Key, name), parameter, context);
        }
    }

    public static class Blender2DExtensions
    {
        public static Option<Blender2D> FindBlender2D(this IAnimationGraph graph, string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));

            return graph.FindDescendantControl<Blender2D>(path);
        }
    }
}
