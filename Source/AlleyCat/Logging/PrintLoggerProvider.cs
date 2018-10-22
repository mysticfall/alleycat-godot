using AlleyCat.Autowire;
using EnsureThat;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [Singleton(typeof(ILoggerProvider))]
    public class PrintLoggerProvider : AutowiredNode, ILoggerProvider
    {
        protected IMemoryCache Cache { get; }

        public PrintLoggerProvider() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public PrintLoggerProvider(IMemoryCache cache)
        {
            Ensure.That(cache, nameof(cache)).IsNotNull();

            Cache = cache;
        }

        public ILogger CreateLogger(string categoryName)
        {
            Ensure.That(categoryName, nameof(categoryName)).IsNotNull();

            return Cache.GetOrCreate(categoryName, _ => new PrintLogger(categoryName));
        }

        public override void _ExitTree() => Cache.Dispose();
    }
}
