using AlleyCat.Action;
using AlleyCat.Autowire;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    [AutowireContext]
    public abstract class InputActionFactory<T> : ActionFactory<T> where T : InputAction
    {
        [Node]
        public Option<ITriggerInput> Input { get; set; }

        protected override Validation<string, T> CreateService(
            string key, string displayName, ILoggerFactory loggerFactory)
        {
            return Input
                .ToValidation("Failed to find the input node.")
                .Bind(input => CreateService(key, displayName, input, loggerFactory));
        }

        protected abstract Validation<string, T> CreateService(
            string key, string displayName, ITriggerInput input, ILoggerFactory loggerFactory);
    }
}
