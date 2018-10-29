using System;
using AlleyCat.Common;
using AlleyCat.Game;
using LanguageExt;

namespace AlleyCat.View
{
    public interface IFocusTracker : IView, IActivatable, IValidatable
    {
        float MaxFocalDistance { get; set; }

        Option<IEntity> FocusedObject { get; }

        IObservable<Option<IEntity>> OnFocusChange { get; }
    }
}
