using System;
using System.Collections.Generic;
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
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable
    {
        private static readonly IEqualityComparer<T> DefaultEqualityComparer = EqualityComparer<T>.Default;

        private T _value;

        private bool _isDisposed;

        private Option<Exception> _lastException;

        private Option<Subject<T>> _publisher = Some(_ => new Subject<T>());

        private Option<IDisposable> _sourceConnection;

        public T Value
        {
            get => _value;
            set
            {
                if (HasValue && EqualityComparer.Equals(_value, value)) return;

                HasValue = true;

                _value = value;

                if (_isDisposed) return;

                _publisher.Iter(p => p.OnNext(_value));
            }
        }

        public bool HasValue { get; private set; }

        protected virtual IEqualityComparer<T> EqualityComparer => DefaultEqualityComparer;

        public ReactiveProperty(T initialValue = default)
        {
            HasValue = true;

            _value = initialValue;
        }

        public ReactiveProperty(IObservable<T> source, T initialValue = default)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            HasValue = false;
            Value = initialValue;

            _sourceConnection = Some(source.Subscribe(new ReactivePropertyObserver(this)));
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

            return _publisher.Match(
                p =>
                {
                    var subscription = p.Subscribe(observer);

                    if (HasValue)
                    {
                        observer.OnNext(_value);
                    }

                    return subscription;
                },
                () =>
                {
                    observer.OnCompleted();

                    return Disposable.Empty;
                });
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
            _sourceConnection.Iter(c => c.DisposeQuietly());

            try
            {
                _publisher.Iter(p => p.OnCompleted());
            }
            finally
            {
                _publisher.Iter(p => p.DisposeQuietly());
                _publisher = None;
            }
        }

        public override string ToString() => _value == null ? "(null)" : _value.ToString();

        private class ReactivePropertyObserver : IObserver<T>
        {
            private readonly ReactiveProperty<T> _parent;

            private int _isStopped;

            public ReactivePropertyObserver(ReactiveProperty<T> parent)
            {
                Ensure.That(parent, nameof(parent)).IsNotNull();

                _parent = parent;
            }

            public void OnNext(T value)
            {
                _parent.Value = value;
            }

            public void OnError(Exception error)
            {
                Ensure.That(error, nameof(error)).IsNotNull();

                if (Interlocked.Increment(ref _isStopped) != 1) return;

                _parent._lastException = error;
                _parent._publisher.Iter(p => p.OnError(error));

                _parent.DisposeQuietly();
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) != 1) return;

                var sc = _parent._sourceConnection;

                _parent._sourceConnection = None;

                sc.Iter(c => c.DisposeQuietly());
            }
        }
    }
}
