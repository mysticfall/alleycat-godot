using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character
{
    public class Race : Node, IRace
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;
    }
}
