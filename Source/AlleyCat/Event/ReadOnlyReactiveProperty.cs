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
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IDisposable
    {
        private static readonly IEqualityComparer<T> DefaultEqualityComparer = EqualityComparer<T>.Default;

        private readonly bool _distinctUntilChanged;

        private bool _canPublishValueOnSubscribe;

        private bool _isDisposed;

        private Exception _lastException;

        private T _value;

        private Subject<T> _publisher;

        private IDisposable _sourceConnection;

        private bool _isSourceCompleted;

        public T Value => _value;

        public bool HasValue => _canPublishValueOnSubscribe;

        protected virtual IEqualityComparer<T> EqualityComparer => DefaultEqualityComparer;

        public ReadOnlyReactiveProperty(
            [NotNull] IObservable<T> source,
            T initialValue = default(T),
            bool distinctUntilChanged = true)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            _distinctUntilChanged = distinctUntilChanged;
            _value = initialValue;
            _canPublishValueOnSubscribe = true;
            _sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
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

            if (_isSourceCompleted)
            {
                if (_canPublishValueOnSubscribe)
                {
                    observer.OnNext(_value);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                observer.OnCompleted();

                return Disposable.Empty;
            }


            _publisher = _publisher ?? new Subject<T>();

            var p = _publisher;

            if (p != null)
            {
                var subscription = p.Subscribe(observer);
                if (_canPublishValueOnSubscribe)
                {
                    observer.OnNext(_value); // raise latest value on subscribe
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

        private class ReadOnlyReactivePropertyObserver : IObserver<T>
        {
            private readonly ReadOnlyReactiveProperty<T> _parent;

            private int _isStopped;

            public ReadOnlyReactivePropertyObserver(ReadOnlyReactiveProperty<T> parent)
            {
                _parent = parent;
            }

            public void OnNext(T value)
            {
                if (_parent._distinctUntilChanged && _parent._canPublishValueOnSubscribe)
                {
                    if (_parent.EqualityComparer.Equals(_parent._value, value)) return;

                    _parent._value = value;
                    _parent._publisher?.OnNext(value);
                }
                else
                {
                    _parent._value = value;
                    _parent._canPublishValueOnSubscribe = true;

                    _parent._publisher?.OnNext(value);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref _isStopped) != 1) return;

                _parent._lastException = error;
                _parent._publisher?.OnError(error);

                _parent.Dispose();
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) != 1) return;

                _parent._isSourceCompleted = true;
                _parent._sourceConnection = null;
                _parent._sourceConnection?.Dispose();

                var p = _parent._publisher;

                _parent._publisher = null;

                if (p == null) return;

                try
                {
                    p.OnCompleted();
                }
                finally
                {
                    p.Dispose();
                }
            }
        }
    }
}
