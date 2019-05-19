using System.Linq;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    public static class DelegateObjectExtensions
    {
        private const string DelegateContextKey = "DelegateContext";

        public static void AssignDelegate(this Node node, Node target)
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(target, nameof(target)).IsNotNull();

            node.SetMeta(DelegateContextKey, target.Name);
        }

        public static void ClearDelegate(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            node.SetMeta(DelegateContextKey, null);
        }

        public static bool HasDelegate(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.HasMeta(DelegateContextKey);
        }

        public static Option<Node> FindDelegate(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return Optional(node)
                .Filter(n => n.HasMeta(DelegateContextKey))
                .Bind(n => Optional(n.GetMeta(DelegateContextKey)))
                .OfType<string>()
                .Bind(name => node.GetChildren().OfType<Node>().Find(n => n.Name == name))
                .HeadOrNone();
        }
    }
}
