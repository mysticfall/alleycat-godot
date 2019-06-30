using AlleyCat.Attribute;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public class SpeedAttributeFactory : AttributeFactory<SpeedAttribute>
    {
        protected override Validation<string, SpeedAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            ILoggerFactory loggerFactory)
        {
            return new SpeedAttribute(
                key,
                displayName,
                description,
                icon,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory);
        }
    }
}
