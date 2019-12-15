using System.Linq;
using AlleyCat.Common;
using Godot;
using Godot.Collections;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    public class BoneMorphDefinitionFactory : RangedMorphDefinitionFactory<BoneMorphDefinition>
    {
        [Export]
        public BoneMorphType MorphType { get; set; } = BoneMorphType.Scale;

        [Export]
        public Vector3 Modifier { get; set; }

        [Export]
        public Array<string> Bones { get; set; }

        protected override Validation<string, BoneMorphDefinition> CreateResource(
            string key, string displayName, bool hidden)
        {
            var range = new Range<float>(MinValue, MaxValue, TFloat.Inst);

            return Optional(Bones).Filter(Enumerable.Any)
                .ToValidation("Missing the target material list.")
                .Map(bones =>
                    new BoneMorphDefinition(key, displayName, bones, MorphType, Modifier, range, Default, hidden));
        }
    }
}
