using AlleyCat.Attribute;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class EncumbranceAttributeFactory : AttributeFactory<EncumbranceAttribute>
    {
        protected override Validation<string, EncumbranceAttribute> CreateService(
            string key, string displayName, Option<string> description, ILoggerFactory loggerFactory)
        {
            return new EncumbranceAttribute(
                key, 
                displayName, 
                description,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory);
        }
    }
}
