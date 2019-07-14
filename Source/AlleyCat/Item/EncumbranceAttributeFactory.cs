using AlleyCat.Attribute;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class EncumbranceAttributeFactory : AttributeFactory<EncumbranceAttribute>
    {
        protected override Validation<string, EncumbranceAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory)
        {
            return new EncumbranceAttribute(
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
