using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [Singleton(typeof(ILoggerProvider))]
    public abstract class LoggerProvider : AutowiredNode, ILoggerProvider
    {
        [Export(PropertyHint.Range, "1,20")]
        public int CategorySegments { get; set; } = 1;

        [Export]
        public bool ShowId { get; set; } = true;

        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public ILogger CreateLogger(string categoryName) =>
            _cache.GetOrCreate(categoryName, _ => DoCreateLogger(categoryName));

        protected abstract ILogger DoCreateLogger(string categoryName);

        protected override void PreDestroy()
        {
            _cache.DisposeQuietly();

            base.PreDestroy();
        }
    }
}
