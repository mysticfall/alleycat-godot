using System;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public class NodeDirectory<T> : Directory<T> where T : Node
    {
        public virtual string Delimiter { get; } = "-";

        protected override string GetKey(T item)
        {
            Ensure.Any.IsNotNull(item, nameof(item));

            var index = item.Name.IndexOf(Delimiter, StringComparison.Ordinal);

            if (index <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The specificed node name doesn't contain a valid key: '{item.Name}'.");
            }

            return item.Name.Substring(0, index - 1).Trim();
        }

        [NotNull]
        protected Node WithKey([NotNull] Node node, [NotNull] string key)
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Any.IsNotNull(key, nameof(key));

            node.Name = $"{key} - {node.Name}";

            return node;
        }

        [NotNull]
        protected Node WithoutKey([NotNull] Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var index = node.Name.IndexOf(Delimiter, StringComparison.Ordinal);

            if (index > 0)
            {
                node.Name = node.Name.Substring(index + 1).Trim();
            }

            return node;
        }
    }
}
