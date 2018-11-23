using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character.Morph
{
    public class MorphGroup : GameObject, IMorphGroup
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public IEnumerable<IMorphDefinition> Definitions { get; }

        public MorphGroup(
            string key,
            string displayName,
            IEnumerable<IMorphDefinition> definitions,
            ILogger logger) : base(logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(definitions, nameof(definitions)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            Definitions = definitions.Freeze();
        }
    }
}
