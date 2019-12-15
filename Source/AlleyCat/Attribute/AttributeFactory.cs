using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    [AutowireContext]
    public abstract class AttributeFactory<T> : GameNodeFactory<T> where T : IAttribute
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Export]
        public string Description { get; set; }

        [Export]
        public Texture Icon { get; set; }

        [Export]
        public bool Active { get; set; }

        [Node]
        public IEnumerable<IAttribute> Children { get; set; }

        protected override Validation<string, T> CreateService(ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(() => Name);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);
            var description = Description.TrimToOption().Map(Tr);
            var children = Optional(Children).Flatten().ToMap();

            return CreateService(key, displayName, description, Optional(Icon), children, loggerFactory);
        }

        protected abstract Validation<string, T> CreateService(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            ILoggerFactory loggerFactory);
    }
}
