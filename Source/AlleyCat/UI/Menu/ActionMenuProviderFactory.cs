using AlleyCat.Control;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Menu
{
    public class ActionMenuProviderFactory : PlayerMenuProviderFactory<ActionMenuProvider>
    {
        protected override Validation<string, ActionMenuProvider> CreateService(
            string key, 
            string displayName,
            PlayerControl playerControl,
            ILoggerFactory loggerFactory)
        {
            return new ActionMenuProvider(key, displayName, playerControl, loggerFactory);
        }
    }
}
