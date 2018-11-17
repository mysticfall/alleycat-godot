using AlleyCat.Common;
using Godot;
using LanguageExt;

namespace AlleyCat.Action
{
    public abstract class ActionFactory<T> : GameObjectFactory<T> where T : Action
    {
        [Export]
        public bool Active { get; set; } = true;

        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        protected override Validation<string, T> CreateService()
        {
            var key = Key.TrimToOption().IfNone(GetName);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return CreateService(key, displayName);
        }

        protected abstract Validation<string, T> CreateService(string key, string displayName);
    }
}
