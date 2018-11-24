using System;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Common
{
    public abstract class GameObject : IValidatable, IInitializable, IDisposableCollector, IDisposable
    {
        protected ILogger Logger { get; }

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        public virtual bool Valid => _initialized && !_disposed;

        private bool _initialized;

        private bool _disposed;

        protected GameObject(ILoggerFactory loggerFactory)
        {
            Ensure.That(loggerFactory, nameof(loggerFactory)).IsNotNull();

            Logger = loggerFactory.CreateLogger(GetType());
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
