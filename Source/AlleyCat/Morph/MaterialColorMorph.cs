using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    public class MaterialColorMorph : Morph<Color, MaterialColorMorphDefinition>
    {
        public IMeshObject Parent { get; }

        public IEnumerable<SpatialMaterial> Materials =>
            from mesh in Parent.Meshes
            from target in Targets
            from material in target.FindMaterial(mesh)
            select material;

        public IEnumerable<MaterialTarget> Targets { get; }

        public MaterialColorMorph(
            IMeshObject parent,
            IEnumerable<MaterialTarget> targets,
            MaterialColorMorphDefinition definition,
            ILoggerFactory loggerFactory) : base(definition, loggerFactory)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            Parent = parent;
            Targets = targets?.Freeze();

            Ensure.Enumerable.HasItems(Targets, nameof(targets));
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(Apply, this);
        }

        protected void Apply(Color value) => Materials.Iter(m => m.AlbedoColor = value);
    }
}
