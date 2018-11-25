using System;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public abstract class GameObject : IValidatable, IInitializable, ILoggable, IDisposableCollector, IDisposable
    {
        public ILogger Logger => _logger.Head();

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        public virtual bool Valid => _initialized && !_disposed;

        private bool _initialized;

        private bool _disposed;

        private readonly Option<ILogger> _logger;

        protected GameObject(ILoggerFactory loggerFactory)
        {
            Ensure.That(loggerFactory, nameof(loggerFactory)).IsNotNull();

            _logger = Some(_ => loggerFactory.CreateLogger(this.GetLogCategory()));
        }

        public void Collect(IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            _disposables += disposable;
        }

        public void Initialize()
        {
            if (_initialized)
            {
                throw new InvalidOperationException("The service has already been initialized.");
            }

            this.LogDebug("Initializing game object.");

            PostConstruct();

            _initialized = true;
        }

        protected virtual void PostConstruct()
        {
        }

        public void Dispose()
        {
            if (_disposed)
            {
                throw new InvalidOperationException("The service has already been disposed.");
            }

            this.LogDebug("Disposing game object.");

            PreDestroy();

            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();

            _disposed = true;
        }

        protected virtual void PreDestroy()
        {
        }
    }
}
