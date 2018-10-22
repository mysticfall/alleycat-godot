using System;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class RangeEventTracker : EventTracker<Range>
    {
        private const string SignalValueChange = "value_changed";

        public IObservable<ValueChangedEvent> OnValueChange
        {
            get
            {
                if (_onValueChange.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalValueChange, this, nameof(FireOnValueChange)));

                    _onValueChange = new Subject<ValueChangedEvent>();
                }

                return _onValueChange.Head();
            }
        }

        private Option<Subject<ValueChangedEvent>> _onValueChange = None;

        [UsedImplicitly]
        private void FireOnValueChange(float value) => _onValueChange
            .SelectMany(o => Parent, (o, p) => (o, e: new ValueChangedEvent(value, p)))
            .Iter(t => t.o.OnNext(t.e));

        protected override void Disconnect(Range parent)
        {
            base.Disconnect(parent);

            _onValueChange.Iter(p =>
            {
                parent.Disconnect(SignalValueChange, this, nameof(FireOnValueChange));
                p.DisposeQuietly();
            });

            _onValueChange = None;
        }
    }
}
