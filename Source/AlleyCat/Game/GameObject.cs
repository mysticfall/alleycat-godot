using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    [NonInjectable]
    public abstract class GameObject : ReactiveObject, IGameObject, ILoggable
    {
        public ILogger Logger => Some(_ => LoggerFactory.CreateLogger(this.GetLogCategory())).Head();

        public ILoggerFactory LoggerFactory { get; }

        public virtual bool Valid => _valid;

        private readonly Option<ILogger> _logger;

        private bool _valid;

        protected GameObject(ILoggerFactory loggerFactory)
        {
            Ensure.That(loggerFactory, nameof(loggerFactory)).IsNotNull();

            LoggerFactory = loggerFactory;
        }

        protected override void PostConstruct()
        {
            this.LogDebug("Initializing game object.");

            base.PostConstruct();

            Initialized
                .CombineLatest(Disposed, (i, d) => i && !d)
                .Subscribe(v => _valid = v, this);
        }

        protected override void PreDestroy()
        {
            this.LogDebug("Disposing game object.");

            base.PreDestroy();
        }
    }
}
