using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Control
{
    public class PlayerActionDelegateFactory : PlayerActionFactory<PlayerActionDelegate>
    {
        [Export]
        public string Action { get; set; }

        protected override Validation<string, PlayerActionDelegate> CreateService(
            string key, string displayName, ITriggerInput input, IPlayerControl control)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(input, nameof(input)).IsNotNull();
            Ensure.That(control, nameof(control)).IsNotNull();

            return Action.TrimToOption()
                .ToValidation("Missing the action name.")
                .Map(action => new PlayerActionDelegate(key, displayName, action, input, control));
        }
    }
}
