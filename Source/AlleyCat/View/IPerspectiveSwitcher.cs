using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.View
{
    public interface IPerspectiveSwitcher : IFocusTracker
    {
        IEnumerable<IPerspectiveView> Perspectives { get; }

        IPerspectiveView Perspective { get; set; }

        IObservable<IPerspectiveView> OnPerspectiveChange { get; }
    }

    public static class PerspectiveSwitcherExtensions
    {
        [CanBeNull]
        public static T SwitchPerspective<T>([NotNull] this IPerspectiveSwitcher control) where T : IPerspectiveView
        {
            Ensure.Any.IsNotNull(control, nameof(control));

            var perspective = control.Perspectives.OfType<T>().FirstOrDefault(p => p.Valid && !p.Active);

            if (perspective != null)
            {
                control.Perspective = perspective;
            }

            return perspective;
        }

        [CanBeNull]
        public static IFirstPersonView SwitchToFirstPerson([NotNull] this IPerspectiveSwitcher control) =>
            SwitchPerspective<IFirstPersonView>(control);

        [CanBeNull]
        public static IThirdPersonView SwitchToThirdPerson([NotNull] this IPerspectiveSwitcher control) =>
            SwitchPerspective<IThirdPersonView>(control);
    }
}
