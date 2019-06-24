using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Game;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    [AutowireContext]
    public class AttributeSetFactory : GameObjectFactory<AttributeSet>
    {
        [Service]
        public IEnumerable<IAttribute> Attributes { get; set; }

        protected override Validation<string, AttributeSet> CreateService(ILoggerFactory loggerFactory)
        {
            var attributes = Optional(Attributes).Flatten();

            return new AttributeSet(attributes, loggerFactory);
        }
    }
}
