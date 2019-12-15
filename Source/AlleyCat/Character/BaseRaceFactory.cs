using System.Collections.Generic;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Item;
using Godot;
using LanguageExt;

namespace AlleyCat.Character
{
    public abstract class BaseRaceFactory<T> : GameResourceFactory<T> where T : Race
    {
        [Export]
        public string DisplayName { get; set; }

        [Export]
        public IEnumerable<EquipmentSlotFactory> EquipmentSlots { get; set; }

        protected override Validation<string, T> CreateResource()
        {
            var key = this.GetKey();
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return CreateResource(key, displayName);
        }

        protected abstract Validation<string, T> CreateResource(string key, string displayName);
    }
}
