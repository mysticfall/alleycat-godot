using System;
using Godot;

namespace AlleyCat.Event
{
    public interface IInputSource
    {
        IObservable<InputEvent> OnInput { get; }

        IObservable<InputEvent> OnUnhandledInput { get; }

        void SetInputAsHandled();
    }
}
