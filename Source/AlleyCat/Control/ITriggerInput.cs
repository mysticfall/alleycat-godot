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
        public static Option<ITriggerInput> FindTrigger(this IInputBindings bindings, string key = "Value")
        {
            Ensure.That(bindings, nameof(bindings)).IsNotNull();

            return bindings.Inputs.Find(key).OfType<ITriggerInput>().HeadOrNone();
        }
    }
}
