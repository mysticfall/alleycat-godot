using System;
using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Mesh;
using EnsureThat;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public class BlendShapeMorphDefinition : RangedMorphDefinition
    {
        public IEnumerable<string> BlendShapes { get; }

        public BlendShapeMorphMode MorphMode { get; }

        public BlendShapeMorphDefinition(
            string key,
            string displayName,
            IEnumerable<string> blendShapes,
            BlendShapeMorphMode morphMode,
            Range<float> range,
            float defaultValue,
            bool hidden,
            ILoggerFactory loggerFactory) : base(key, displayName, range, defaultValue, hidden, loggerFactory)
        {
            BlendShapes = blendShapes?.Freeze();

            Ensure.Enumerable.HasItems(BlendShapes, nameof(blendShapes));

            MorphMode = morphMode;
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

            return new BlendShapeMorph(meshObject, this, LoggerFactory);
        }
    }
}
