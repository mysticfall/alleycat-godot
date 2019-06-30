using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class VariableAttributeFactory : AttributeFactory<VariableAttribute>
    {
        [Export]
        public float InitialValue { get; set; }

        protected override Validation<string, VariableAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            ILoggerFactory loggerFactory)
        {
            return new VariableAttribute(
                key,
                displayName,
                description,
                icon,
                InitialValue,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory);
        }
    }
}
