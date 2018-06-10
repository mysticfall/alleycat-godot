using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public class MorphGroup : IdentifiableDirectory<IMorphDefinition>, IMorphGroup
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;
    }
}
