using System;
using AlleyCat.Common;
using AlleyCat.View;

namespace AlleyCat.Control
{
    public interface IFocusTracker : IView, IActivatable, IValidatable
    {
        float MaxFocalDistance { get; set; }

        IEntity FocusedObject { get; }

        IObservable<IEntity> OnFocusChange { get; }
    }
}
