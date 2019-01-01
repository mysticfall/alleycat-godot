using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using LanguageExt;

namespace AlleyCat.Event
{
    public class ReactiveObject : IReactiveObject, IInitializable
    {
        public IObservable<bool> Initialized => _initialized.AsObservable();

        public IObservable<bool> Disposed => _disposed.AsObservable();

        private readonly BehaviorSubject<bool> _initialized;

        private readonly BehaviorSubject<bool> _disposed;

        private Lst<IDisposable> _disposables;

        public ReactiveObject()
        {
            _initialized = CreateSubject(false);
            _disposed = CreateSubject(false);
        }

        public void Initialize()
        {
            if (_initialized.Value)
            {
                throw new InvalidOperationException("The object has already been initialized.");
            }

            PostConstruct();

            _initialized.OnNext(true);
        }

        protected virtual void PostConstruct()
        {
        }

        public BehaviorSubject<T> CreateSubject<T>(T initialValue)
        {
            var subject = new BehaviorSubject<T>(initialValue);

            _disposables += new ManagedSubject<T>(subject);

            return subject;
        }

        public ISubject<T> CreateSubject<T>()
        {
            var subject = new Subject<T>();

            _disposables += new ManagedSubject<T>(subject);

            return subject;
        }

        public void Dispose()
        {
            if (_disposed.Value)
            {
                throw new InvalidOperationException("The object has already been disposed.");
            }

            PreDestroy();

            _disposed.OnNext(true);

            _disposables.Iter(s => s.Dispose());
            _disposables = _disposables.Clear();
        }

        protected virtual void PreDestroy()
        {
        }
    }

    internal struct ManagedSubject<T> : IDisposable
    {
        private readonly ISubject<T> _subject;

        public ManagedSubject(ISubject<T> subject)
        {
            Debug.Assert(subject != null, "subject != null");

            _subject = subject;
        }

        public void Dispose() => _subject.CompleteAndDispose();
    }
}
