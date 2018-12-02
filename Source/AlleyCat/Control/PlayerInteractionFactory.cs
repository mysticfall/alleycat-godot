using System;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class PlayerInteractionFactory : PlayerActionFactory<PlayerInteraction>
    {
        protected override Validation<string, PlayerInteraction> CreateService(
            string key, 
            string displayName, 
            Func<Option<IPlayerControl>> control, 
            ITriggerInput input, 
            ILoggerFactory loggerFactory)
        {
            return new PlayerInteraction(key, displayName, control, input, Active, loggerFactory);
        }
    }
}
