using AlleyCat.Autowire;
using EnsureThat;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [Singleton(typeof(ILoggerProvider))]
    public class ConsoleLoggerProvider : AutowiredNode, ILoggerProvider
    {
        [NotNull]
        protected IMemoryCache Cache { get; }

        public ConsoleLoggerProvider() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public ConsoleLoggerProvider([NotNull] IMemoryCache cache)
        {
            Ensure.Any.IsNotNull(cache, nameof(cache));

            Cache = cache;
        }

        [NotNull]
        public ILogger CreateLogger([NotNull] string categoryName)
        {
            Ensure.String.IsNotNullOrWhiteSpace(categoryName, nameof(categoryName));

            return Cache.GetOrCreate(categoryName, _ => new ConsoleLogger(categoryName));
        }

        public override void _ExitTree() => Cache.Dispose();
    }
}
