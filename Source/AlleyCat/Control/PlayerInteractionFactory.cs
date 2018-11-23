using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public class PlayerInteractionFactory : PlayerActionFactory<PlayerInteraction>
    {
        protected override Validation<string, PlayerInteraction> CreateService(
            string key, 
            string displayName, 
            Option<IPlayerControl> control, 
            ITriggerInput input, 
            ILogger logger)
        {
            return new PlayerInteraction(key, displayName, control, input, Active, logger);
        }
    }
}
