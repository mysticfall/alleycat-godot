using System.Diagnostics;
using System.Reflection;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public static class NodeExtensions
    {
        private static readonly IMemoryCache AttributeCache = new MemoryCache(new MemoryCacheOptions());

        public static IAutowireContext GetRootContext(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return AutowireContext.GetOrCreate(node.GetTree().Root);
        }

        public static IAutowireContext GetAutowireContext(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

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

        public static void Autowire(this Node node) => Autowire(node, None);

        internal static void Autowire(this Node node, Option<AutowireContext> context)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var target = context.IfNone((AutowireContext) GetAutowireContext(node));

            Debug.Assert(target != null, "target != null");

            target.Register(node);

            if (target.Node == node)
            {
                target.Initialize();
            }
        }
    }
}
