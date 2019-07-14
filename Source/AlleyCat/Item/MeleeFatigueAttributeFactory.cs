using AlleyCat.Attribute;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class MeleeFatigueAttributeFactory : AttributeFactory<MeleeFatigueAttribute>
    {
        protected override Validation<string, MeleeFatigueAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory)
        {
            return new MeleeFatigueAttribute(
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
