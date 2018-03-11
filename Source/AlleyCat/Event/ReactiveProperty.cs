using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    /// <summary>
    /// Adapted from UniRx (https://github.com/neuecc/UniRx).
    /// </summary>
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable
    {
        private static readonly IEqualityComparer<T> DefaultEqualityComparer = EqualityComparer<T>.Default;

        private bool _isDisposed;

        private Exception _lastException;

        private T _value;

        private Subject<T> _publisher;

        private IDisposable _sourceConnection;

        public T Value
        {
            get => _value;
            set
            {
                if (!HasValue)
                {
                    HasValue = true;

                    _value = value;

                    if (_isDisposed) return;

                    _publisher?.OnNext(_value);

                    return;
                }

                if (!EqualityComparer.Equals(_value, value))
                {
                    _value = value;

                    if (_isDisposed) return;

                    _publisher?.OnNext(_value);
                }
            }
        }

        public bool HasValue { get; private set; }

        protected virtual IEqualityComparer<T> EqualityComparer => DefaultEqualityComparer;

        public ReactiveProperty(T initialValue = default(T))
        {
            HasValue = true;
            
            _value = initialValue;
        }

        public ReactiveProperty([NotNull] IObservable<T> source, T initialValue = default(T))
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            HasValue = false;
            Value = initialValue;

            _sourceConnection = source.Subscribe(new ReactivePropertyObserver(this));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Ensure.Any.IsNotNull(observer, nameof(observer));

            if (_lastException != null)
            {
                observer.OnError(_lastException);
                return Disposable.Empty;
            }

            if (_isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            _publisher = _publisher ?? new Subject<T>();

            var p = _publisher;

            if (p != null)
            {
                var subscription = p.Subscribe(observer);

                if (HasValue)
                {
                    observer.OnNext(_value);
                }

                return subscription;
            }

            observer.OnCompleted();

            return Disposable.Empty;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            _isDisposed = true;

            _sourceConnection?.Dispose();
            _sourceConnection = null;

            try
            {
                _publisher?.OnCompleted();
            }
            finally
            {
                _publisher?.Dispose();
                _publisher = null;
            }
        }

        public override string ToString() => _value == null ? "(null)" : _value.ToString();

        private class ReactivePropertyObserver : IObserver<T>
        {
            private readonly ReactiveProperty<T> _parent;

            private int _isStopped;

            public ReactivePropertyObserver(ReactiveProperty<T> parent)
            {
                _parent = parent;
            }

            public void OnNext(T value)
            {
                _parent.Value = value;
            }

            public void OnError(Exception error)
            {
                Ensure.Any.IsNotNull(error, nameof(error));

                if (Interlocked.Increment(ref _isStopped) != 1) return;

                _parent._lastException = error;
                _parent._publisher?.OnError(error);

                _parent.Dispose();
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) != 1) return;

                var sc = _parent._sourceConnection;

                _parent._sourceConnection = null;

                sc?.Dispose();
            }
        }
    }
}
