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
    public class ButtonEventTracker : EventTracker<Button>
    {
        private const string SignalItemSelect = "pressed";

        public IObservable<ButtonPressedEvent> OnPressed
        {
            get
            {
                if (_onPressed.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalItemSelect, this, nameof(FireOnPress)));

                    _onPressed = new Subject<ButtonPressedEvent>();
                }

                return _onPressed.Head();
            }
        }

        private Option<Subject<ButtonPressedEvent>> _onPressed;

        [UsedImplicitly]
        private void FireOnPress() => _onPressed
            .SelectMany(o => Parent, (o, p) => (o, e: new ButtonPressedEvent(p)))
            .Iter(t => t.o.OnNext(t.e));

        protected override void Disconnect(Button parent)
        {
            base.Disconnect(parent);

            _onPressed.Iter(p =>
            {
                parent.Disconnect(SignalItemSelect, this, nameof(FireOnPress));
                p.DisposeQuietly();
            });

            _onPressed = None;
        }
    }
}
