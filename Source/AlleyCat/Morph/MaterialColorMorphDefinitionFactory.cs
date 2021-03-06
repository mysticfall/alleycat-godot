using System.Linq;
using Godot;
using Godot.Collections;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Morph
{
    public class MaterialColorMorphDefinitionFactory : ColorMorphDefinitionFactory<MaterialColorMorphDefinition>
    {
        [Export]
        public Array<string> Targets { get; set; }

        protected override Validation<string, MaterialColorMorphDefinition> CreateResource(
            string key, string displayName, bool hidden)
        {
            return
                from targets in Optional(Targets).Filter(Enumerable.Any).Map(v => v.Map(MaterialTarget.Create))
                    .ToValidation("Missing the target material list.")
                select new MaterialColorMorphDefinition(key, displayName, targets, Default, UseAlpha, hidden);
        }
    }
}
