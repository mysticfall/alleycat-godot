using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Logging;
using AlleyCat.Mesh;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    public class MaterialColorMorph : Morph<Color, MaterialColorMorphDefinition>
    {
        public IMeshObject Parent { get; }

        public IEnumerable<Material> Materials =>
            from mesh in Parent.Meshes
            from target in Definition.Targets
            from material in target.FindMaterial(mesh)
            select material;

        public MaterialColorMorph(
            IMeshObject parent,
            MaterialColorMorphDefinition definition,
            ILoggerFactory loggerFactory) : base(definition, loggerFactory)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            Parent = parent;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(Apply, this);
        }

        protected void Apply(Color value)
        {
            Materials.Iter(m =>
            {
                switch (m)
                {
                    case SpatialMaterial spatial:
                        spatial.AlbedoColor = value;
                        break;
                    case ShaderMaterial shader:
                        shader.SetShaderParam("albedo", value);
                        break;
                }
            });
        }
    }
}
