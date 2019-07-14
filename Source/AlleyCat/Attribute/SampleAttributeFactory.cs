using System;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class SampleAttributeFactory : AttributeFactory<SampleAttribute>
    {
        [Export]
        public string Target { get; set; }

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Export(PropertyHint.Range, "0.1,60")]
        public float Period { get; set; } = 0.1f;

        protected override Validation<string, SampleAttribute> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory)
        {
            var target = Target.TrimToOption().ToValidation("Missing target attribute name.");

            return target.Map(t => new SampleAttribute(
                key,
                displayName,
                description,
                icon,
                t,
                TimeSpan.FromSeconds(Math.Max(Period, 0.1f)),
                ProcessMode,
                this,
                children,
                Active,
                loggerFactory));
        }
    }
}
