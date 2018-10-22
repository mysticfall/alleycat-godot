using AlleyCat.Common;
using Godot;

namespace AlleyCat.Character.Morph
{
    public class MorphGroup : IdentifiableDirectory<IMorphDefinition>, IMorphGroup
    {
        public string Key => _key.TrimToOption().IfNone(Name);

        public virtual string DisplayName => _displayName.TrimToOption().Map(Tr).IfNone(Key);

        [Export] private string _key;

        [Export] private string _displayName;
    }
}
