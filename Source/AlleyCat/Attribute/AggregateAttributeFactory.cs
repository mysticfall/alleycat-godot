using System.Collections.Generic;
using AlleyCat.Autowire;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    [AutowireContext]
    public class AggregateAttributeFactory : AttributeFactory<AggregateAttribute>
    {
        [Service(local: true)]
        public IEnumerable<IAttribute> Attributes { get; set; }

        protected override Validation<string, AggregateAttribute> CreateService(
            string key, 
            string displayName, 
            Option<string> description,
            Option<Texture> icon,
            ILoggerFactory loggerFactory)
        {
            return new AggregateAttribute(
                key,
                displayName,
                description,
                icon,
                Attributes,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory);
        }
    }
}
