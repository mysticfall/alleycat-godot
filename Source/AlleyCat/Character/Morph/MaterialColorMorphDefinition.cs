using System;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Gen = System.Collections.Generic;

namespace AlleyCat.Character.Morph
{
    public class MaterialColorMorphDefinition : ColorMorphDefinition
    {
        [Export, UsedImplicitly]
        public string Mesh { get; private set; }

        public Gen.IEnumerable<string> Materials => _materials?.Split(';') ?? Enumerable.Empty<string>();

        [Export, UsedImplicitly] private string _materials;

        public override IMorph CreateMorph(IMorphable morphable)
        {
            Ensure.Any.IsNotNull(morphable, nameof(morphable));

            if (!(morphable is IMeshObject meshObject))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    "The specified morphable does not implement IMeshObject interface.");
            }

            var mesh = meshObject.Meshes.FirstOrDefault(m => m.Name == Mesh);

            if (!(mesh?.Mesh is ArrayMesh arrayMesh))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    $"The specified morphable does not contain mesh: '{Mesh}'.");
            }

            var count = arrayMesh.GetSurfaceCount();
            var indexes = new Gen.Dictionary<string, int>(count);

            for (var i = 0; i < count; i++)
            {
                indexes.Add(arrayMesh.SurfaceGetName(i), i);
            }

            var materials = Materials.Select(name => indexes[name]);

            return new MaterialColorMorph(mesh, materials, this);
        }
    }
}
