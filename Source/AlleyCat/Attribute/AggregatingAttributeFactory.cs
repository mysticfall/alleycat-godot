using System.Collections.Generic;
using AlleyCat.Autowire;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    [AutowireContext]
    public class AggregatingAttributeFactory : AttributeFactory<AggregatingAttribute>
    {
        [Service(local: true)]
        public IEnumerable<IAttribute> Attributes { get; set; }

        protected override Validation<string, AggregatingAttribute> CreateService(
            string key, string displayName, Option<string> description, ILoggerFactory loggerFactory)
        {
            return new AggregatingAttribute(
                key,
                displayName,
                description,
                Attributes,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory);
        }
    }
}
