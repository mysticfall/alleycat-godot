using Godot;
using Godot.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public abstract class EquipmentConfigurationFactory<T> : SlotConfigurationFactory<T>
        where T : EquipmentConfiguration
    {
        [Export]
        public bool Active { get; set; }

        [Export]
        public Mesh Mesh { get; set; }

        [Export]
        public Godot.Animation EquipAnimation { get; set; }

        [Export]
        public Godot.Animation UnequipAnimation { get; set; }

        [Export]
        public Godot.Animation Animation { get; set; }

        [Export]
        public string AnimationBlend { get; set; }

        [Export(PropertyHint.ExpRange, "0,10")]
        public float AnimationTransition { get; set; }

        [Export]
        public Array<string> Tags { get; set; }

        protected override Validation<string, T> CreateService(
            string key, 
            string slot, 
            Set<string> additionalSlots, 
            ILoggerFactory loggerFactory)
        {
            var tags = toSet(Optional(Tags).Flatten());

            return CreateService(key, slot, additionalSlots, tags, loggerFactory);
        }

        protected abstract Validation<string, T> CreateService(
            string key, 
            string slot, 
            Set<string> additionalSlots, 
            Set<string> tags, 
            ILoggerFactory loggerFactory);
    }
}
