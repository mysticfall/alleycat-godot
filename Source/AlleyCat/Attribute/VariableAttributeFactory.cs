using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class VariableAttributeFactory : AttributeFactory<VariableAttribute>
    {
        [Export]
        public bool Active { get; set; }

        [Export]
        public float InitialValue { get; set; }

        protected override Validation<string, VariableAttribute> CreateService(
            string key, string displayName, Option<string> description, ILoggerFactory loggerFactory)
        {
            return new VariableAttribute(
                key,
                displayName,
                description,
                InitialValue,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory);
        }
    }
}
