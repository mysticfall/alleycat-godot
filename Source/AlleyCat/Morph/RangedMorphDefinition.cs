using AlleyCat.Common;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public abstract class RangedMorphDefinition : MorphDefinition<float>
    {
        public Range<float> Range { get; }

        protected RangedMorphDefinition(
            string key,
            string displayName,
            Range<float> range,
            float defaultValue,
            bool hidden,
            ILoggerFactory loggerFactory) : base(key, displayName, defaultValue, hidden, loggerFactory)
        {
            Range = range;
        }
    }
}
