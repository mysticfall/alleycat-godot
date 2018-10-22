using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Character.Morph
{
    public class MaterialColorMorph : Morph<Color, MaterialColorMorphDefinition>
    {
        public MeshInstance Mesh { get; }

        public IEnumerable<SpatialMaterial> Materials => SurfaceIndexes
            .Bind(i => Mesh.FindSurfaceMaterial(i).AsEnumerable())
            .OfType<SpatialMaterial>();

        public IEnumerable<int> SurfaceIndexes { get; }

        public MaterialColorMorph(MeshInstance mesh,
            IEnumerable<int> indexes,
            MaterialColorMorphDefinition definition) : base(definition)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNull();

            Mesh = mesh;
            SurfaceIndexes = indexes.Freeze();

            if (!SurfaceIndexes.Any())
            {
                throw new ArgumentOutOfRangeException(
                    nameof(definition),
                    $"The morph '{Definition.Key}' does not have any target material defined.");
            }
        }

        protected override void Apply(Color value) => Materials.Iter(m => m.AlbedoColor = value);
    }
}
