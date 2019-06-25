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
            string key, string displayName, Option<string> description, ILoggerFactory loggerFactory)
        {
            return new ConstantAttribute(
                key,
                displayName,
                description,
                Value,
                Active,
                loggerFactory);
        }
    }
}
