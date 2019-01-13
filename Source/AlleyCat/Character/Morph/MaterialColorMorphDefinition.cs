using System;
using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character.Morph
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
            ILoggerFactory loggerFactory) : base(key, displayName, defaultValue, useAlpha, loggerFactory)
        {
            Targets = targets?.Freeze();

            Ensure.Enumerable.HasItems(Targets, nameof(targets));
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

            return new MaterialColorMorph(meshObject, Targets, this, LoggerFactory);
        }
    }
}
