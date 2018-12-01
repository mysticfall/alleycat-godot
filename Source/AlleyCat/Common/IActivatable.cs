using System;
using AlleyCat.Autowire;
using EnsureThat;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IActivatable
    {
        bool Active { get; set; }

        IObservable<bool> OnActiveStateChange { get; }
    }

    public static class ActivatableExtensions
    {
        public static void Activate(this IActivatable activatable)
        {
            Ensure.That(activatable, nameof(activatable)).IsNotNull();

            activatable.Active = true;
        }

        public static void Deactivate(this IActivatable activatable)
        {
            Ensure.That(activatable, nameof(activatable)).IsNotNull();

            activatable.Active = false;
        }
    }
}
