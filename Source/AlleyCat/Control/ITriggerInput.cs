using System.Linq;
using AlleyCat.Control.Generic;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Control
{
    public interface ITriggerInput : IInput<bool>
    {
    }

    public static class TriggerInputExtensions
    {
        public static Option<ITriggerInput> FindTrigger(this InputBindings bindings, string key = "Value")
        {
            Ensure.That(bindings, nameof(bindings)).IsNotNull();
            Ensure.That(key, nameof(key)).IsNotNull();

            return bindings.TryGetValue(key).OfType<ITriggerInput>().HeadOrNone();
        }
    }
}
