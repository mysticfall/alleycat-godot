using System;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public class ControlEventTracker : EventTracker<Godot.Control>
    {
        private const string SignalMouseEnter = "mouse_entered";

        private const string SignalMouseExit = "mouse_exited";

        [NotNull]
        public IObservable<MouseEnteredEvent> OnMouseEnter
        {
            get
            {
                if (_onMouseEnter == null)
                {
                    Parent.Connect(SignalMouseEnter, this, "FireOnMouseEnter");

                    _onMouseEnter = new Subject<MouseEnteredEvent>();
                }

                return _onMouseEnter;
            }
        }

        [NotNull]
        public IObservable<MouseExitedEvent> OnMouseExit
        {
            get
            {
                if (_onMouseExit == null)
                {
                    Parent.Connect(SignalMouseExit, this, "FireOnMouseExit");

                    _onMouseExit = new Subject<MouseExitedEvent>();
                }

                return _onMouseExit;
            }
        }

        private Subject<MouseEnteredEvent> _onMouseEnter;

        private Subject<MouseExitedEvent> _onMouseExit;

        [UsedImplicitly]
        private void FireOnMouseEnter() => _onMouseEnter?.OnNext(new MouseEnteredEvent(Parent));

        [UsedImplicitly]
        private void FireOnMouseExit() => _onMouseExit?.OnNext(new MouseExitedEvent(Parent));

        protected override void Disconnect(Godot.Control parent)
        {
            base.Disconnect(parent);

            Ensure.Any.IsNotNull(parent, nameof(parent));

            if (_onMouseEnter != null)
            {
                parent.Disconnect(SignalMouseEnter, this, "FireOnMouseEnter");

                _onMouseEnter.Dispose();
                _onMouseEnter = null;
            }

            if (_onMouseExit != null)
            {
                parent.Disconnect(SignalMouseExit, this, "FireOnMouseExit");

                _onMouseExit.Dispose();
                _onMouseExit = null;
            }
        }
    }
}
