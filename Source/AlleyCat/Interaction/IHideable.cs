using System;
using System.Reactive;
using JetBrains.Annotations;

namespace AlleyCat.Interaction
{
    public interface IHideable
    {
        bool Visible { get; }

        void Show();

        void Hide();

        [NotNull]
        IObservable<Unit> OnShow { get; }

        [NotNull]
        IObservable<Unit> OnHide { get; }

        [NotNull]
        IObservable<bool> OnVisibilityChange { get; }
    }
}
