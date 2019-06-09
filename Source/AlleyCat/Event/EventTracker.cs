using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using Object = Godot.Object;

namespace AlleyCat.Event
{
    internal class EventTracker : Object
    {
        public const string TargetMethod = nameof(OnNext);

        public IObservable<IEnumerable<object>> OnSignal => _subject.AsObservable();

        private readonly Subject<IEnumerable<object>> _subject = new Subject<IEnumerable<object>>();

        private void OnNext() => _subject.OnNext(Enumerable.Empty<object>());

        private void OnNext(object arg) => _subject.OnNext(List(arg));

        private void OnNext(object arg1, object arg2) => _subject.OnNext(List(arg1, arg2));

        private void OnNext(object arg1, object arg2, object arg3) => _subject.OnNext(List(arg1, arg2, arg3));

        protected override void Dispose(bool disposing)
        {
            _subject.CompleteAndDispose();

            base.Dispose(disposing);
        }
    }
}
