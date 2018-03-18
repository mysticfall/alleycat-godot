using System;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public class RangeEventTracker : EventTracker<Range>
    {
        private const string SignalValueChange = "value_changed";

        [NotNull]
        public IObservable<ValueChangedEvent> OnValueChange
        {
            get
            {
                if (_onValueChange == null)
                {
                    Parent.Connect(SignalValueChange, this, "FireOnValueChange");

                    _onValueChange = new Subject<ValueChangedEvent>();
                }

                return _onValueChange;
            }
        }

        private Subject<ValueChangedEvent> _onValueChange;

        [UsedImplicitly]
        private void FireOnValueChange(float value) =>
            _onValueChange?.OnNext(new ValueChangedEvent(value, Parent));

        protected override void Disconnect(Range parent)
        {
            base.Disconnect(parent);

            Ensure.Any.IsNotNull(parent, nameof(parent));

            if (_onValueChange != null)
            {
                parent.Disconnect(SignalValueChange, this, "FireOnValueChange");

                _onValueChange.Dispose();
                _onValueChange = null;
            }
        }
    }
}
