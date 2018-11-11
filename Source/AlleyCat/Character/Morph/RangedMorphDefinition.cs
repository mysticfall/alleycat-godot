using AlleyCat.Common;

namespace AlleyCat.Character.Morph
{
    public abstract class RangedMorphDefinition : MorphDefinition<float>
    {
        public Range<float> Range { get; }

        protected RangedMorphDefinition(
            string key, 
            string displayName, 
            Range<float> range,
            float defaultValue) : base(key, displayName, defaultValue)
        {
            Range = range;
        }
    }
}
