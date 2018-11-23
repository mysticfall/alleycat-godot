using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    public class MaterialColorMorphDefinition : ColorMorphDefinition
    {
        public string Mesh { get; }

        public IEnumerable<string> Materials { get; }

        public MaterialColorMorphDefinition(
            string key,
            string displayName,
            string mesh,
            IEnumerable<string> materials,
            Color defaultValue,
            bool useAlpha,
            ILogger logger) : base(key, displayName, defaultValue, useAlpha, logger)
        {
            Ensure.That(mesh, nameof(mesh)).IsNotNullOrEmpty();

            Mesh = mesh;
            Materials = materials?.Freeze();

            Ensure.Enumerable.HasItems(Materials, nameof(materials));
        }

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
