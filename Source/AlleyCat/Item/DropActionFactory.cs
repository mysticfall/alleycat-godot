using AlleyCat.Action;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Item
{
    public class DropActionFactory : ActionFactory<DropAction>
    {
        protected override Validation<string, DropAction> CreateService(
            string key, string displayName, ILogger logger)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return new DropAction(key, displayName, Active, logger);
        }
    }
}
