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
    public class ColorPickerButtonEventTracker : EventTracker<ColorPickerButton>
    {
        private const string SignalColorChange = "color_changed";

        public IObservable<ColorChangedEvent> OnColorChange
        {
            get
            {
                if (_onColorChange.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalColorChange, this, nameof(FireOnColorChange)));

                    _onColorChange = new Subject<ColorChangedEvent>();
                }

                return _onColorChange.Head();
            }
        }

        private Option<Subject<ColorChangedEvent>> _onColorChange;

        [UsedImplicitly]
        private void FireOnColorChange(Color color) => _onColorChange
            .SelectMany(o => Parent, (o, p) => (o, e: new ColorChangedEvent(color, p)))
            .Iter(t => t.o.OnNext(t.e));

        protected override void Disconnect(ColorPickerButton parent)
        {
            base.Disconnect(parent);

            _onColorChange.Iter(p =>
            {
                parent.Disconnect(SignalColorChange, this, nameof(FireOnColorChange));
                p.DisposeQuietly();
            });

            _onColorChange = None;
        }
    }
}
