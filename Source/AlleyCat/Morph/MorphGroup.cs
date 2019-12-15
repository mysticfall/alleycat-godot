using System.Collections;
using System.Collections.Generic;
using EnsureThat;

namespace AlleyCat.Morph
{
    public class MorphGroup : IMorphGroup
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public IEnumerator<IMorphDefinition> GetEnumerator() => _definitions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _definitions.GetEnumerator();

        private readonly IEnumerable<IMorphDefinition> _definitions;

        public MorphGroup(string key, string displayName, IEnumerable<IMorphDefinition> definitions)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(definitions, nameof(definitions)).IsNotNull();

            Key = key;
            DisplayName = displayName;

            _definitions = definitions.Freeze();
        }
    }
}
