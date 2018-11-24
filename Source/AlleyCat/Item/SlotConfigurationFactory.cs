using AlleyCat.Common;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class SlotConfigurationFactory<T> : GameObjectFactory<T> where T : ISlotConfiguration
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string Slot { get; set; }

        [Export, UsedImplicitly]
        public Array<string> AdditionalSlots { get; set; }

        protected override Validation<string, T> CreateService(ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var slot = Slot.TrimToOption().IfNone(key);

            return CreateService(key, slot, toSet(AdditionalSlots), loggerFactory);
        }

        protected abstract Validation<string, T> CreateService(
            string key, string slot, Set<string> additionalSlots, ILoggerFactory loggerFactory);
    }
}
