using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using Godot.Collections;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    public class MaterialColorMorphDefinition : ColorMorphDefinition
    {
        public string Mesh => _mesh.TrimToOption().Head();

        public IEnumerable<string> Materials => _materials ?? Enumerable.Empty<string>();

        [Export] private string _mesh;

        [Export] private Array<string> _materials;

        public override IMorph CreateMorph(IMorphable morphable)
        {
            Ensure.That(morphable, nameof(morphable)).IsNotNull();

            if (!(morphable is IMeshObject meshObject))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    "The specified morphable does not implement IMeshObject interface.");
            }

            var instance = meshObject.Meshes.Find(m => m.Name == Mesh).IfNone(() =>
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    $"The specified morphable does not contain mesh: '{Mesh}'."));

            var mesh = Optional(instance.Mesh).OfType<ArrayMesh>().HeadOrNone();

            if (!mesh.Any())
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    $"The specified morphable does not contain mesh: '{Mesh}'.");
            }

            var indexes = toMap(mesh
                .Bind(m => Enumerable.Range(0, m.GetSurfaceCount()).Map(m.SurfaceGetName))
                .Map((index, name) => (name, index)));

            var materials = Materials.Select(name => indexes[name]);

            return new MaterialColorMorph(instance, materials, this);
        }
    }
}
