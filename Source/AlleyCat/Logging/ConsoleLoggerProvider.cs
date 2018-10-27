using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.UI.Console;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [Singleton(typeof(ILoggerProvider))]
    public class ConsoleLoggerProvider : AutowiredNode, ILoggerProvider
    {
        public DebugConsole Console => (DebugConsole) _console;

        protected IMemoryCache Cache { get; }

        [Service] private Option<DebugConsole> _console;

        public ConsoleLoggerProvider() : this(new MemoryCache(new MemoryCacheOptions()))
        {
        }

        public ConsoleLoggerProvider(IMemoryCache cache)
        {
            Ensure.That(cache, nameof(cache)).IsNotNull();

            Cache = cache;
        }

        public ILogger CreateLogger(string categoryName)
        {
            Ensure.That(categoryName, nameof(categoryName)).IsNotNull();

            return Cache.GetOrCreate(categoryName, _ => new ConsoleLogger(categoryName, Console));
        }

        public override void _ExitTree() => Cache.DisposeQuietly();
    }
}
