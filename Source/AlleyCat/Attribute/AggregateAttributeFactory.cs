using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    [AutowireContext]
    public class AggregateAttributeFactory : AttributeFactory<AggregateAttribute>
    {
        [Node]
        public IEnumerable<IAttribute> Sources { get; set; }

        protected override Validation<string, AggregateAttribute> CreateService(
            string key, 
            string displayName, 
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory)
        {
            var sources = Optional(Sources).Flatten().Freeze();

            return new AggregateAttribute(
                key,
                displayName,
                description,
                icon,
                sources,
                children.AddRange(sources.ToMap()),
                Active,
                loggerFactory);
        }
    }
}
