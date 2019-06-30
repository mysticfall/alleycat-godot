using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class DelegateAttributeFactory : AttributeFactory<DelegateAttribute>
    {
        [Export]
        public string Target { get; set; }

        protected override Validation<string, DelegateAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            ILoggerFactory loggerFactory)
        {
            var target = Target.TrimToOption().ToValidation("Missing target attribute name.");

            return target.Map(t => new DelegateAttribute(
                key,
                displayName,
                description,
                icon,
                t,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory));
        }
    }
}
