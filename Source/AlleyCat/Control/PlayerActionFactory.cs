using AlleyCat.Common;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public abstract class PlayerActionFactory<T> : InputActionFactory<T> where T : PlayerAction
    {
        protected override Validation<string, T> CreateService(
            string key, string displayName, ITriggerInput input, ILogger logger)
        {
            var control = Optional(_ => this.FindClosestAncestor<IPlayerControl>()).Flatten();

            return CreateService(key, displayName, control, input, logger);
        }

        protected abstract Validation<string, T> CreateService(
            string key, string displayName, Option<IPlayerControl> control, ITriggerInput input, ILogger logger);
    }
}
