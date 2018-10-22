using System;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class ControlEventTracker : EventTracker<Godot.Control>
    {
        private const string SignalMouseEnter = "mouse_entered";

        private const string SignalMouseExit = "mouse_exited";

        public IObservable<MouseEnteredEvent> OnMouseEnter
        {
            get
            {
                if (_onMouseEnter.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalMouseEnter, this, nameof(FireOnMouseEnter)));

                    _onMouseEnter = new Subject<MouseEnteredEvent>();
                }

                return _onMouseEnter.Head();
            }
        }

        public IObservable<MouseExitedEvent> OnMouseExit
        {
            get
            {
                if (_onMouseExit.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalMouseExit, this, nameof(FireOnMouseExit)));

                    _onMouseExit = new Subject<MouseExitedEvent>();
                }

                return _onMouseExit.Head();
            }
        }

        private Option<Subject<MouseEnteredEvent>> _onMouseEnter = None;

        private Option<Subject<MouseExitedEvent>> _onMouseExit = None;

        [UsedImplicitly]
        private void FireOnMouseEnter() => _onMouseEnter
            .SelectMany(o => Parent, (o, p) => (o, e: new MouseEnteredEvent(p)))
            .Iter(t => t.o.OnNext(t.e));

        [UsedImplicitly]
        private void FireOnMouseExit() => _onMouseExit
            .SelectMany(o => Parent, (o, p) => (o, e: new MouseExitedEvent(p)))
            .Iter(t => t.o.OnNext(t.e));

        protected override void Disconnect(Godot.Control parent)
        {
            base.Disconnect(parent);

            _onMouseEnter.Iter(p =>
            {
                parent.Disconnect(SignalMouseEnter, this, nameof(FireOnMouseEnter));
                p.DisposeQuietly();
            });

            _onMouseEnter = None;

            _onMouseExit.Iter(p =>
            {
                parent.Disconnect(SignalMouseExit, this, nameof(FireOnMouseExit));
                p.DisposeQuietly();
            });

            _onMouseExit = None;
        }
    }
}
