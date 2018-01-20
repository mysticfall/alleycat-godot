using System;
using System.Reflection;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;

namespace AlleyCat.Autowire
{
    public static class NodeExtensions
    {
        private const string ContextName = "AutowireContext";

        private static IAutowireContext _rootContext;

        private static readonly IMemoryCache AttributeCache = new MemoryCache(new MemoryCacheOptions());

        [NotNull]
        public static IAutowireContext GetRootContext([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            if (_rootContext != null)
            {
                return _rootContext;
            }

            var viewport = node.GetTree().Root;

            return _rootContext = GetLocalContext(viewport);
        }

        public static IAutowireContext GetLocalContext([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            return node.GetOrCreateNode(
                ContextName, _ => new AutowireContext {Name = ContextName});
        }

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
                    return GetLocalContext(current);
                }

                current = current.GetParent();
            }

            return GetRootContext(node);
        }

        public static void Prewire([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var context = GetAutowireContext(node);

            if (context == null)
            {
                throw new InvalidOperationException(
                    $"No IAutowireContext found for node: '{node.Name}'.");
            }

            context.Prewire(node);
        }

        public static void Postwire([NotNull] this Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var context = GetAutowireContext(node);

            if (context == null)
            {
                throw new InvalidOperationException(
                    $"No IAutowireContext found for node: '{node.Name}'.");
            }

            context.Postwire(node);
        }
    }
}
