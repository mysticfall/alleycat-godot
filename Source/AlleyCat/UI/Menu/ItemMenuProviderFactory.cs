using AlleyCat.Control;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Menu
{
    public class ItemMenuProviderFactory : PlayerMenuProviderFactory<ItemMenuProvider>
    {
        protected override Validation<string, ItemMenuProvider> CreateService(
            string key,
            string displayName,
            PlayerControl playerControl,
            ILoggerFactory loggerFactory)
        {
            return new ItemMenuProvider(key, displayName, this, playerControl, loggerFactory);
        }
    }
}
