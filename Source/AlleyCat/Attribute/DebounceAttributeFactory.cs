using System;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class DebounceAttributeFactory : AttributeFactory<DebounceAttribute>
    {
        [Export]
        public string Target { get; set; }

        [Export]
        public string Gate { get; set; }

        [Export(PropertyHint.Range, "0.1,60")]
        public float Period { get; set; } = 0.1f;

        [Export]
        public bool AsRatio { get; set; }

        protected override Validation<string, DebounceAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory)
        {
            var target = Target.TrimToOption().ToValidation("Missing target attribute name.");

            return target.Map(t => new DebounceAttribute(
                key,
                displayName,
                description,
                icon,
                t,
                Gate.TrimToOption(),
                TimeSpan.FromSeconds(Math.Max(Period, 0.1f)),
                AsRatio,
                children,
                Active,
                loggerFactory));
        }
    }
}
