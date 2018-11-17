using EnsureThat;
using LanguageExt;

namespace AlleyCat.Control
{
    public class PlayerInteractionFactory : PlayerActionFactory<PlayerInteraction>
    {
        protected override Validation<string, PlayerInteraction> CreateService(
            string key, string displayName, Option<IPlayerControl> control, ITriggerInput input)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(input, nameof(input)).IsNotNull();

            return new PlayerInteraction(key, displayName, control, input);
        }
    }
}
