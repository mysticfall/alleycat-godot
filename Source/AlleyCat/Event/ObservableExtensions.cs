using System;
using System.Reactive;
using System.Reactive.Linq;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public static class ObservableExtensions
    {
        [NotNull]
        public static IObservable<Unit> AsUnitObservable<T>([NotNull] this IObservable<T> observable)
        {
            Ensure.Any.IsNotNull(observable, nameof(observable));

            return observable.Select(_ => Unit.Default);
        }
    }
}
