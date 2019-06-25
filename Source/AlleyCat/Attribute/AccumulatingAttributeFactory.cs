using System;
using AlleyCat.Autowire;
using AlleyCat.Event;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class AccumulatingAttributeFactory : AttributeFactory<AccumulatingAttribute>
    {
        [Export]
        public float InitialValue { get; set; }

        [Node]
        public Option<IAttribute> Generator { get; set; }

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Export(PropertyHint.Range, "0.1,60")]
        public float Period { get; set; } = 0.1f;

        protected override Validation<string, AccumulatingAttribute> CreateService(
            string key, string displayName, Option<string> description, ILoggerFactory loggerFactory)
        {
            return new AccumulatingAttribute(
                key,
                displayName,
                description,
                InitialValue,
                Min,
                Max,
                Modifier,
                Generator,
                TimeSpan.FromSeconds(Math.Max(Period, 0.1f)),
                ProcessMode,
                this,
                Active,
                loggerFactory);
        }
    }
}
