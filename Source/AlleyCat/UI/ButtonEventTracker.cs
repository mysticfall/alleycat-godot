using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class ButtonEventTracker : EventTracker<Button>
    {
        private const string SignalPressed = "pressed";

        private const string SignalButtonUp = "button_up";

        private const string SignalButtonDown = "button_down";

        public IObservable<ButtonPressedEvent> OnPressed
        {
            get
            {
                if (_onPressed.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalPressed, this, nameof(FireOnPress)));

                    _onPressed = new Subject<ButtonPressedEvent>();
                }

                return _onPressed.Head().AsObservable();
            }
        }

        public IObservable<ButtonUpEvent> OnButtonUp
        {
            get
            {
                if (_onButtonUp.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalButtonUp, this, nameof(FireOnButtonUp)));

                    _onButtonUp = new Subject<ButtonUpEvent>();
                }

                return _onButtonUp.Head().AsObservable();
            }
        }

        public IObservable<ButtonDownEvent> OnButtonDown
        {
            get
            {
                if (_onButtonDown.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalButtonDown, this, nameof(FireOnButtonDown)));

                    _onButtonDown = new Subject<ButtonDownEvent>();
                }

                return _onButtonDown.Head().AsObservable();
            }
        }

        private Option<Subject<ButtonPressedEvent>> _onPressed;

        private Option<Subject<ButtonUpEvent>> _onButtonUp;

        private Option<Subject<ButtonDownEvent>> _onButtonDown;

        [UsedImplicitly]
        private void FireOnPress() => _onPressed
            .SelectMany(o => Parent, (o, p) => (o, e: new ButtonPressedEvent(p)))
            .Iter(t => t.o.OnNext(t.e));

        [UsedImplicitly]
        private void FireOnButtonUp() => _onButtonUp
            .SelectMany(o => Parent, (o, p) => (o, e: new ButtonUpEvent(p)))
            .Iter(t => t.o.OnNext(t.e));

        [UsedImplicitly]
        private void FireOnButtonDown() => _onButtonDown
            .SelectMany(o => Parent, (o, p) => (o, e: new ButtonDownEvent(p)))
            .Iter(t => t.o.OnNext(t.e));

        protected override void Disconnect(Button parent)
        {
            base.Disconnect(parent);

            _onPressed.Iter(p =>
            {
                parent.Disconnect(SignalPressed, this, nameof(FireOnPress));
                p.DisposeQuietly();
            });

            _onButtonUp.Iter(p =>
            {
                parent.Disconnect(SignalButtonUp, this, nameof(FireOnButtonUp));
                p.DisposeQuietly();
            });

            _onButtonDown.Iter(p =>
            {
                parent.Disconnect(SignalButtonDown, this, nameof(FireOnButtonDown));
                p.DisposeQuietly();
            });

            _onPressed = None;
            _onButtonUp = None;
            _onButtonDown = None;
        }
    }
}
