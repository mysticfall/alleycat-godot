using System;
using AlleyCat.Common;

namespace AlleyCat.View
{
    public interface IFocusTracker : IView, IActivatable, IValidatable
    {
        float MaxFocalDistance { get; set; }

        IEntity FocusedObject { get; }

        IObservable<IEntity> OnFocusChange { get; }
    }
}
