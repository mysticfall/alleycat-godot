using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public abstract class MorphDefinition<T> : Node, IMorphDefinition
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr($"morph.{Group.Key}.{Key}");

        [Export, UsedImplicitly]
        public T Default { get; private set; }

        public IMorphGroup Group => (IMorphGroup) GetParent();

        [Export, UsedImplicitly] private string _key;

        public abstract IMorph CreateMorph(IMorphable morphable);
    }
}
