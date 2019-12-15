using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    [NonInjectable]
    public abstract class GameNode : ReactiveObject, IGameNode, ILoggable
    {
        public ILogger Logger => _logger.Invoke();

        public ILoggerFactory LoggerFactory { get; }

        public virtual bool Valid => _valid;

        private readonly Func<ILogger> _logger;

        private bool _valid;

        protected GameNode(ILoggerFactory loggerFactory)
        {
            Ensure.That(loggerFactory, nameof(loggerFactory)).IsNotNull();

            LoggerFactory = loggerFactory;

            _logger = memo(() => LoggerFactory.CreateLogger(this.GetLogCategory()));
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

        public override string ToString() =>
            this is INamed named ? $"{named.DisplayName}({named.Key})" : $"{GetType().Name}({GetHashCode()})";
    }
}
