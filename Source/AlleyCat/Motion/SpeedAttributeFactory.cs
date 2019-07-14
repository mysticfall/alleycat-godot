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
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory)
        {
            return new SpeedAttribute(
                key,
                displayName,
                description,
                icon,
                children,
                Active,
                loggerFactory);
        }
    }
}
