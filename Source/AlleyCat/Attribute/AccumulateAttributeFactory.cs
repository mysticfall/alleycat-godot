using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    public class AccumulateAttributeFactory : AttributeFactory<AccumulateAttribute>
    {
        [Export]
        public float InitialValue { get; set; }

        [Node]
        public IEnumerable<IAttribute> Sources { get; set; }

        protected override Validation<string, AccumulateAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory)
        {
            var sources = Optional(Sources).Flatten().Freeze();

            return new AccumulateAttribute(
                key,
                displayName,
                description,
                icon,
                InitialValue,
                sources,
                children.AddRange(sources.ToMap()),
                Active,
                loggerFactory);
        }
    }
}
