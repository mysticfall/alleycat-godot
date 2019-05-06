using EnsureThat;
using LanguageExt;

namespace AlleyCat.UI.Menu
{
    public class MenuItem : IMenuItem
    {
        public string Key { get; }

        public string DisplayName { get; }

        public object Model { get; }

        public Option<IMenuItem> Parent { get; }

        public MenuItem(string key, string displayName, object model, Option<IMenuItem> parent)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(displayName, nameof(displayName)).IsNotNull();
            Ensure.That(model, nameof(model)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            Model = model;
            Parent = parent;
        }

        public override string ToString() => $"MenuItem(Key = '{Key}', Model = {Model})";
    }
}
