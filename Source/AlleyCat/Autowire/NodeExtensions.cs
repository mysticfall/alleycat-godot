using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public static class NodeExtensions
    {
        private static IAutowireContext _rootContext;

        [CanBeNull]
        public static IAutowireContext GetAutowireContext([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var current = node;

            while (current != null)
            {
                if (current is IAutowireContext context)
                {
                    return context;
                }

                current = current.GetParent();
            }

            return _rootContext ?? (_rootContext = node.GetTree().Root.GetOrCreateChild(
                       _ => AutowireContext.CreateRootContext()));
        }

        public static void Autowire([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var context = GetAutowireContext(node);

            if (context == null)
            {
                throw new InvalidOperationException(
                    $"No IAutowireContext found for node: '{node.Name}'.");
            }

            context.Resolve(node);
        }
    }
}
