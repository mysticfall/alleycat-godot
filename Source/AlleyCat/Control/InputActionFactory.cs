using AlleyCat.Action;
using AlleyCat.Autowire;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Control
{
    [AutowireContext]
    public abstract class InputActionFactory<T> : ActionFactory<T> where T : InputAction
    {
        [Node]
        public Option<ITriggerInput> Input { get; set; }

        protected override Validation<string, T> CreateService(string key, string displayName)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            return Input
                .ToValidation("Failed to find the input node.")
                .Bind(input => CreateService(key, displayName, input));
        }

        protected abstract Validation<string, T> CreateService(string key, string displayName, ITriggerInput input);
    }
}
