using System.Linq;
using AlleyCat.Common;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class EquipmentConfigurationFactory<T> : SlotConfigurationFactory<T>, ITaggable
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

        public Set<string> Tags => toSet(GetGroups().OfType<string>());
    }
}
