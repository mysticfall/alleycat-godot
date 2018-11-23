using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class PlayerActionDelegateFactory : PlayerActionFactory<PlayerActionDelegate>
    {
        [Export]
        public string Action { get; set; }

        protected override Validation<string, PlayerActionDelegate> CreateService(
            string key, string displayName, Option<IPlayerControl> control, ITriggerInput input, ILogger logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(input, nameof(input)).IsNotNull();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return Action.TrimToOption()
                .ToValidation("Missing the action name.")
                .Map(action => new PlayerActionDelegate(key, displayName, action, control, input, Active, logger));
        }
    }
}
