using System;
using System.Collections.Generic;
using AlleyCat.Mesh;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public class MaterialColorMorphDefinition : ColorMorphDefinition
    {
        public IEnumerable<MaterialTarget> Targets { get; }

        public MaterialColorMorphDefinition(
            string key,
            string displayName,
            IEnumerable<MaterialTarget> targets,
            Color defaultValue,
            bool useAlpha,
            bool hidden) : base(key, displayName, defaultValue, useAlpha, hidden)
        {
            Targets = targets?.Freeze();

            Ensure.Enumerable.HasItems(Targets, nameof(targets));
        }

        public override IMorph CreateMorph(IMorphable morphable, ILoggerFactory loggerFactory)
        {
            Ensure.That(morphable, nameof(morphable)).IsNotNull();

            if (!(morphable is IMeshObject meshObject))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    "The specified morphable does not implement IMeshObject interface.");
            }

            return new MaterialColorMorph(meshObject, this, loggerFactory);
        }
    }
}
