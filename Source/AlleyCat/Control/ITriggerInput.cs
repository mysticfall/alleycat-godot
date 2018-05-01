using AlleyCat.Control.Generic;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface ITriggerInput : IInput<bool>
    {
    }

    public static class TriggerInputExtensions
    {
        [CanBeNull]
        public static ITriggerInput GetTrigger([NotNull] this InputBindings bindings, [NotNull] string key = "Value")
        {
            Ensure.Any.IsNotNull(bindings, nameof(bindings));
            Ensure.Any.IsNotNull(key, nameof(key));

            return bindings.ContainsKey(key) ? bindings[key] as ITriggerInput : null;
        }
    }
}
