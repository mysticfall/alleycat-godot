using Microsoft.Extensions.Logging;

namespace AlleyCat.Character.Morph
{
    public abstract class RangedMorph<T> : Morph<float, T> where T : RangedMorphDefinition
    {
        public override float Value
        {
            get => base.Value;
            set => base.Value = Definition.Range.Clamp(value);
        }

        protected RangedMorph(T definition, ILoggerFactory loggerFactory) : base(definition, loggerFactory)
        {
        }
    }
}
