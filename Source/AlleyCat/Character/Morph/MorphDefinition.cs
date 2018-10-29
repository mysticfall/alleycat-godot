using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public abstract class MorphDefinition<T> : Node, IMorphDefinition
    {
        public string Key => _key.TrimToOption().IfNone(GetName);

        public virtual string DisplayName => _displayName.TrimToOption().Map(Tr).IfNone(() => Key);

        [Export, UsedImplicitly]
        public T Default { get; private set; }

        public IMorphGroup Group => (IMorphGroup) GetParent();

        [Export] private string _key;

        [Export] private string _displayName;

        public abstract IMorph CreateMorph(IMorphable morphable);
    }
}
