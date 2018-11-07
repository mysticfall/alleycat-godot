using AlleyCat.Common;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;
using LanguageExt;
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

        protected override Validation<string, T> CreateService()
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var slot = Slot.TrimToOption().IfNone(key);

            return CreateService(key, slot, toSet(AdditionalSlots));
        }

        protected abstract Validation<string, T> CreateService(string key, string slot, Set<string> additionalSlots);
    }
}
