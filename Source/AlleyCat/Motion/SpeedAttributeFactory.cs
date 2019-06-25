using AlleyCat.Attribute;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public class SpeedAttributeFactory : AttributeFactory<SpeedAttribute>
    {
        protected override Validation<string, SpeedAttribute> CreateService(
            string key, string displayName, Option<string> description, ILoggerFactory loggerFactory)
        {
            return new SpeedAttribute(
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
