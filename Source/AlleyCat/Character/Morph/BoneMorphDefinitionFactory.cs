using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using Godot.Collections;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    public class BoneMorphDefinitionFactory : RangedMorphDefinitionFactory<BoneMorphDefinition>
    {
        [Export]
        public BoneMorphType MorphType { get; set; } = BoneMorphType.Scale;

        [Export]
        public Vector3 Modifier { get; set; }

        [Export]
        public Array<string> Bones { get; set; }

        protected override Validation<string, BoneMorphDefinition> CreateService(string key, string displayName)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            var range = new Range<float>(MinValue, MaxValue);

            return Optional(Bones).Filter(Enumerable.Any)
                .ToValidation("Missing the target material list.")
                .Map(bones => new BoneMorphDefinition(key, displayName, bones, MorphType, Modifier, range, Default));
        }
    }
}
