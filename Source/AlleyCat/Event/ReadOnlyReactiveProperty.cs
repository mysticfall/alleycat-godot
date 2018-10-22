using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Event
{
    /// <summary>
    /// Adapted from UniRx (https://github.com/neuecc/UniRx).
    /// Copyright (c) 2018 Yoshifumi Kawai
    /// </summary>
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IDisposable
    {
        private static readonly IEqualityComparer<T> DefaultEqualityComparer = EqualityComparer<T>.Default;

        private readonly bool _distinctUntilChanged;

        private bool _isDisposed;

        private Option<Exception> _lastException = None;

        private T _value;

        private Option<Subject<T>> _publisher = Some(_ => new Subject<T>());

        private readonly IDisposable _sourceConnection;

        private bool _isSourceCompleted;

        public T Value => _value;

        public bool HasValue { get; private set; }

        protected virtual IEqualityComparer<T> EqualityComparer => DefaultEqualityComparer;

        public ReadOnlyReactiveProperty(
            IObservable<T> source,
            T initialValue = default,
            bool distinctUntilChanged = true)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            HasValue = true;

            _distinctUntilChanged = distinctUntilChanged;
            _value = initialValue;
            _sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Ensure.That(observer, nameof(observer)).IsNotNull();

            if (_lastException.IsSome)
            {
                _lastException.Iter(observer.OnError);

                return Disposable.Empty;
            }

            if (_isDisposed)
            {
                observer.OnCompleted();

                return Disposable.Empty;
            }

            if (_isSourceCompleted)
            {
                if (HasValue)
                {
                    observer.OnNext(_value);
                    observer.OnCompleted();

                    return Disposable.Empty;
                }

                observer.OnCompleted();

                return Disposable.Empty;
            }

            return _publisher.Match(
                p =>
                {
                    var subscription = p.Subscribe(observer);

                    if (HasValue)
                    {
                        observer.OnNext(_value); // raise latest value on subscribe
                    }

                    return subscription;
                },
                () =>
                {
                    observer.OnCompleted();

                    return Disposable.Empty;
                }
            );
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
            _sourceConnection.DisposeQuietly();

            try
            {
                _publisher.Iter(p => p.OnCompleted());
            }
            finally
            {
                _publisher.Iter(p => p.DisposeQuietly());
            }
        }

        public override string ToString() => _value == null ? "(null)" : _value.ToString();

        private class ReadOnlyReactivePropertyObserver : IObserver<T>
        {
            private readonly ReadOnlyReactiveProperty<T> _parent;

            private int _isStopped;

            public ReadOnlyReactivePropertyObserver(ReadOnlyReactiveProperty<T> parent)
            {
                Ensure.That(parent, nameof(parent)).IsNotNull();

                _parent = parent;
            }

            public void OnNext(T value)
            {
                if (_parent._distinctUntilChanged && _parent.HasValue)
                {
                    if (_parent.EqualityComparer.Equals(_parent._value, value)) return;

                    _parent._value = value;
                    _parent._publisher.Iter(p => p.OnNext(value));
                }
                else
                {
                    _parent._value = value;
                    _parent.HasValue = true;

                    _parent._publisher.Iter(p => p.OnNext(value));
                }
            }

            public void OnError(Exception error)
            {
                Debug.Assert(error != null, "error != null");

                if (Interlocked.Increment(ref _isStopped) != 1) return;

                _parent._lastException = error;
                _parent._publisher.Iter(p => p.OnError(error));

                _parent.DisposeQuietly();
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) != 1) return;

                _parent._isSourceCompleted = true;
                _parent._sourceConnection.DisposeQuietly();

                _parent._publisher.Iter(p =>
                {
                    try
                    {
                        p.OnCompleted();
                    }
                    finally
                    {
                        p.DisposeQuietly();
                    }
                });
            }
        }
    }
}
