using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Condition.Generic;
using AlleyCat.Game;
using Godot;
using LanguageExt;

namespace AlleyCat.Item
{
    public abstract class SlotFactory<T> : GameResourceFactory<T> where T : Slot
    {
        [Export]
        public string DisplayName { get; set; }

        [Node]
        public Option<ICondition<ISlotItem>> AllowedFor { get; set; }

        protected override Validation<string, T> CreateResource()
        {
            var key = this.GetKey();
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return CreateResource(key, displayName);
        }

        protected abstract Validation<string, T> CreateResource(string key, string displayName);
    }
}
