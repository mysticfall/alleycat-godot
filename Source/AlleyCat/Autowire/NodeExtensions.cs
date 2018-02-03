using System;
using System.Reflection;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;

namespace AlleyCat.Autowire
{
    public static class NodeExtensions
    {
        private static readonly IMemoryCache AttributeCache = new MemoryCache(new MemoryCacheOptions());

        [NotNull]
        public static IAutowireContext GetRootContext([NotNull] this Node node) =>
            AutowireContext.GetOrCreate(node.GetTree().Root);

        [NotNull]
        public static IAutowireContext GetAutowireContext([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var current = node;

            while (current != null)
            {
                var type = current.GetType();

                var attribute = AttributeCache.GetOrCreate(
                    type, _ => type.GetCustomAttribute<AutowireContextAttribute>(true));

                if (attribute != null)
                {
                    return AutowireContext.GetOrCreate(current);
                }

                current = current.GetParent();
            }

            return GetRootContext(node);
        }

        public static void Autowire([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            if (!(GetAutowireContext(node) is AutowireContext context))
            {
                throw new InvalidOperationException(
                    $"No AutowireContext found for node: '{node.Name}'.");
            }

            context.Register(node);

            if (context.Node == node)
            {
                context.Initialize();
            }
        }
    }
}
