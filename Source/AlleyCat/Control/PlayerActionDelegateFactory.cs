using System;
using AlleyCat.Common;
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
            string key,
            string displayName,
            Func<Option<IPlayerControl>> control,
            ITriggerInput input,
            ILoggerFactory loggerFactory)
        {
            return Action.TrimToOption()
                .ToValidation("Missing the action name.")
                .Map(action => new PlayerActionDelegate(
                    key, displayName, action, control, input, Active, loggerFactory));
        }
    }
}
