using AlleyCat.Common;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character.Morph
{
    public abstract class RangedMorphDefinition : MorphDefinition<float>
    {
        public Range<float> Range { get; }

        protected RangedMorphDefinition(
            string key,
            string displayName,
            Range<float> range,
            float defaultValue,
            ILogger logger) : base(key, displayName, defaultValue, logger)
        {
            Range = range;
        }
    }
}
