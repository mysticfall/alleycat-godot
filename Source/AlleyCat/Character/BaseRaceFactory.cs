using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Item;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class BaseRaceFactory<T> : GameObjectFactory<T> where T : Race
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Service]
        public IEnumerable<EquipmentSlot> EquipmentSlots { get; set; } = Seq<EquipmentSlot>();

        protected override Validation<string, T> CreateService(ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return CreateService(key, displayName, loggerFactory);
        }

        protected abstract Validation<string, T> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory);
    }
}
