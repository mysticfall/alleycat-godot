using AlleyCat.Action;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Item
{
    public class DropActionFactory : ActionFactory<DropAction>
    {
        protected override Validation<string, DropAction> CreateService(string key, string displayName)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            return new DropAction(key, displayName, Active);
        }
    }
}
