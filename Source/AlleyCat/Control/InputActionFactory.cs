using AlleyCat.Action;
using AlleyCat.Autowire;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    [AutowireContext]
    public abstract class InputActionFactory<T> : ActionFactory<T> where T : InputAction
    {
        [Node]
        public Option<ITriggerInput> Input { get; set; }

        protected override Validation<string, T> CreateService(string key, string displayName, ILogger logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return Input
                .ToValidation("Failed to find the input node.")
                .Bind(input => CreateService(key, displayName, input, logger));
        }

        protected abstract Validation<string, T> CreateService(
            string key, string displayName, ITriggerInput input, ILogger logger);
    }
}
