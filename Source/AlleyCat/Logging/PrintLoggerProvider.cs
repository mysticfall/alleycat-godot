using AlleyCat.Autowire;
using AlleyCat.Common;
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

        public ILogger CreateLogger(string categoryName) =>
            Cache.GetOrCreate(categoryName, _ => new PrintLogger(categoryName));

        protected override void PreDestroy()
        {
            Cache.DisposeQuietly();

            base.PreDestroy();
        }
    }
}
