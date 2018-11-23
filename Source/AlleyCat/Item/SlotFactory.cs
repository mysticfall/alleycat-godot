using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Condition.Generic;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public abstract class SlotFactory<T> : GameObjectFactory<T> where T : Slot
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Node]
        public Option<ICondition<ISlotItem>> AllowedFor { get; set; }

        protected override Validation<string, T> CreateService(ILogger logger)
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return CreateService(key, displayName, logger);
        }

        protected abstract Validation<string, T> CreateService(string key, string displayName, ILogger logger);
    }
}
