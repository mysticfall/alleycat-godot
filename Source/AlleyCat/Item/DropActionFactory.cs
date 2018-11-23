using AlleyCat.Action;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class DropActionFactory : ActionFactory<DropAction>
    {
        protected override Validation<string, DropAction> CreateService(
            string key, string displayName, ILogger logger)
        {
            return new DropAction(key, displayName, Active, logger);
        }
    }
}
