using System;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public class ColorPickerButtonEventTracker : EventTracker<ColorPickerButton>
    {
        private const string SignalColorChange = "color_changed";

        [NotNull]
        public IObservable<ColorChangedEvent> OnColorChange
        {
            get
            {
                if (_onColorChange == null)
                {
                    Parent.Connect(SignalColorChange, this, "FireOnColorChange");

                    _onColorChange = new Subject<ColorChangedEvent>();
                }

                return _onColorChange;
            }
        }

        private Subject<ColorChangedEvent> _onColorChange;

        [UsedImplicitly]
        private void FireOnColorChange(Color color) => _onColorChange?.OnNext(new ColorChangedEvent(color, Parent));

        protected override void Disconnect(ColorPickerButton parent)
        {
            base.Disconnect(parent);

            Ensure.Any.IsNotNull(parent, nameof(parent));

            if (_onColorChange != null)
            {
                parent.Disconnect(SignalColorChange, this, "FireOnColorChange");

                _onColorChange.Dispose();
                _onColorChange = null;
            }
        }
    }
}
