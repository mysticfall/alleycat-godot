using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class ConstantAttributeFactory : AttributeFactory<ConstantAttribute>
    {
        [Export]
        public float Value { get; set; }

        protected override Validation<string, ConstantAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            ILoggerFactory loggerFactory)
        {
            return new ConstantAttribute(
                key,
                displayName,
                description,
                icon,
                Value,
                Active,
                loggerFactory);
        }
    }
}
