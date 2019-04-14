using AlleyCat.Game;
using EnsureThat;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Morph
{
    public abstract class MorphDefinition<T> : GameObject, IMorphDefinition
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public T Default { get; }

        public bool Hidden { get; }

        protected MorphDefinition(
            string key,
            string displayName,
            T defaultValue,
            bool hidden,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            Key = key;
            DisplayName = displayName;
            Default = defaultValue;
            Hidden = hidden;
        }

        public abstract IMorph CreateMorph(IMorphable morphable);
    }
}
