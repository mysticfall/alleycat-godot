using AlleyCat.Autowire;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Control
{
    public abstract class PlayerActionFactory<T> : InputActionFactory<T> where T : PlayerAction
    {
        [Service]
        private Option<IPlayerControl> PlayerControl { get; set; }

        protected override Validation<string, T> CreateService(string key, string displayName, ITriggerInput input)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(input, nameof(input)).IsNotNull();

            return PlayerControl
                .ToValidation("Failed to find the player control component.")
                .Bind(control => CreateService(key, displayName, input, control));
        }

        protected abstract Validation<string, T> CreateService(
            string key, string displayName, ITriggerInput input, IPlayerControl control);
    }
}
