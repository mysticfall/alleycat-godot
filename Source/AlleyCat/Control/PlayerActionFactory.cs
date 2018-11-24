using AlleyCat.Common;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public abstract class PlayerActionFactory<T> : InputActionFactory<T> where T : PlayerAction
    {
        protected override Validation<string, T> CreateService(
            string key, string displayName, ITriggerInput input, ILoggerFactory loggerFactory)
        {
            var control = Optional(_ => this.FindClosestAncestor<IPlayerControl>()).Flatten();

            return CreateService(key, displayName, control, input, loggerFactory);
        }

        protected abstract Validation<string, T> CreateService(
            string key, 
            string displayName, 
            Option<IPlayerControl> control, 
            ITriggerInput input, 
            ILoggerFactory loggerFactory);
    }
}
