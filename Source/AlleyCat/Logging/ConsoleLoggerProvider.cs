using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.UI.Console;
using EnsureThat;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [Singleton(typeof(ILoggerProvider))]
    public class ConsoleLoggerProvider : AutowiredNode, ILoggerProvider
    {
        [Service]
        public DebugConsole Console { get; private set; }

        protected IMemoryCache Cache { get; }

        public ConsoleLoggerProvider() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public ConsoleLoggerProvider(IMemoryCache cache)
        {
            Ensure.That(cache, nameof(cache)).IsNotNull();

            Cache = cache;
        }

        public ILogger CreateLogger(string categoryName) =>
            Cache.GetOrCreate(categoryName, _ => new ConsoleLogger(categoryName, Console));

        protected override void PreDestroy()
        {
            Cache.DisposeQuietly();

            base.PreDestroy();
        }
    }
}
