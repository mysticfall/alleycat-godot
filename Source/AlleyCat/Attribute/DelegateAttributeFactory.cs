using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class DelegateAttributeFactory : AttributeFactory<DelegateAttribute>
    {
        [Export]
        public bool Active { get; set; }

        [Export]
        public string Target { get; set; }

        protected override Validation<string, DelegateAttribute> CreateService(
            string key, string displayName, Option<string> description, ILoggerFactory loggerFactory)
        {
            var target = Target.TrimToOption().ToValidation("Missing target attribute name.");

            return target.Map(t => new DelegateAttribute(
                key,
                displayName,
                description,
                t,
                Min,
                Max,
                Modifier,
                Active,
                loggerFactory));
        }
    }
}
