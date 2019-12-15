using System.Linq;
using AlleyCat.Common;
using Godot;
using Godot.Collections;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    public class BlendShapeMorphDefinitionFactory : RangedMorphDefinitionFactory<BlendShapeMorphDefinition>
    {
        [Export]
        public Array<string> BlendShapes { get; set; }

        [Export]
        public BlendShapeMorphMode MorphMode { get; set; } = BlendShapeMorphMode.Parallel;

        protected override Validation<string, BlendShapeMorphDefinition> CreateResource(
            string key, string displayName, bool hidden)
        {
            var range = new Range<float>(MinValue, MaxValue, TFloat.Inst);

            return
                from blendShapes in Optional(BlendShapes ?? Enumerable.Empty<string>())
                    .Filter(v => v.Any())
                    .ToValidation("Missing the target material list.")
                select new BlendShapeMorphDefinition(
                    key, displayName, blendShapes, MorphMode, range, Default, hidden);
        }
    }
}
