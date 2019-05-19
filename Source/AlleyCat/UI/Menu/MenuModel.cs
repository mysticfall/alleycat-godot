using EnsureThat;
using LanguageExt;

namespace AlleyCat.UI.Menu
{
    public class MenuModel : IMenuModel
    {
        public string Key { get; }

        public string DisplayName { get; }

        public object Model { get; }

        public Option<IMenuModel> Parent { get; }

        public MenuModel(string key, string displayName, object model, Option<IMenuModel> parent)
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
