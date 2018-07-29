using System;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public class ButtonEventTracker : EventTracker<Button>
    {
        private const string SignalItemSelect = "pressed";

        [NotNull]
        public IObservable<ButtonPressedEvent> OnPressed
        {
            get
            {
                if (_onPressed == null)
                {
                    Parent.Connect(SignalItemSelect, this, nameof(FireOnPress));

                    _onPressed = new Subject<ButtonPressedEvent>();
                }

                return _onPressed;
            }
        }

        private Subject<ButtonPressedEvent> _onPressed;

        [UsedImplicitly]
        private void FireOnPress() => _onPressed?.OnNext(new ButtonPressedEvent(Parent));

        protected override void Disconnect(Button parent)
        {
            base.Disconnect(parent);

            Ensure.Any.IsNotNull(parent, nameof(parent));

            if (_onPressed != null)
            {
                parent.Disconnect(SignalItemSelect, this, nameof(FireOnPress));

                _onPressed.Dispose();
                _onPressed = null;
            }
        }
    }
}
