using System;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public interface IActivatable
    {
        bool Active { get; set; }

        IObservable<bool> OnActiveStateChange { get; }
    }

    public static class ActivatableExtensions
    {
        public static void Activate([NotNull] this IActivatable activatable) 
        {
            Ensure.Any.IsNotNull(activatable, nameof(activatable));

            activatable.Active = true;
        }

        public static void Deactivate([NotNull] this IActivatable activatable) 
        {
            Ensure.Any.IsNotNull(activatable, nameof(activatable));

            activatable.Active = false;
        }
    }
}
