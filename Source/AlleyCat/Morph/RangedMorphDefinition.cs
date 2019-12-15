using AlleyCat.Common;

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
            bool hidden) : base(key, displayName, defaultValue, hidden)
        {
            Range = range;
        }
    }
}
