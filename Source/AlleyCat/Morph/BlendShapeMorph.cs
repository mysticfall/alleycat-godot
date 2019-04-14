using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using static AlleyCat.Morph.BlendShapeMorphMode;

namespace AlleyCat.Morph
{
    public class BlendShapeMorph : RangedMorph<BlendShapeMorphDefinition>
    {
        public IMeshObject Parent { get; }

        public IEnumerable<string> BlendShapePaths { get; }

        public BlendShapeMorph(
            IMeshObject parent,
            BlendShapeMorphDefinition definition,
            ILoggerFactory loggerFactory) : base(definition, loggerFactory)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            Parent = parent;
            BlendShapePaths = definition.BlendShapes.Map(v => "blend_shapes/" + v).Freeze();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(Apply, this);
        }

        protected void Apply(float value)
        {
            var count = BlendShapePaths.Count();
            var size = 1f / count;

            float Normalize(float index) => Definition.MorphMode == Parallel
                ? value
                : Mathf.Clamp((value - size * index) / size, 0, 1);

            var values = BlendShapePaths.Map((index, path) => (value: Normalize(index), path));

            Parent.Meshes
                .SelectMany(mesh => values, (m, v) => (mesh: m, v.path, v.value))
                .Iter(v => v.mesh.Set(v.path, v.value));
        }
    }
}
