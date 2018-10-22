using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.View
{
    public interface IPerspectiveSwitcher : IFocusTracker
    {
        IEnumerable<IPerspectiveView> Perspectives { get; }

        Option<IPerspectiveView> Perspective { get; set; }

        IObservable<Option<IPerspectiveView>> OnPerspectiveChange { get; }
    }

    public static class PerspectiveSwitcherExtensions
    {
        public static Option<T> SwitchPerspective<T>(this IPerspectiveSwitcher control)
            where T : IPerspectiveView
        {
            Ensure.That(control, nameof(control)).IsNotNull();

            var perspective = control.Perspectives.OfType<T>().Find(p => p.Valid && !p.Active);

            perspective.Iter(p => control.Perspective = p);

            return perspective;
        }

        public static Option<IFirstPersonView> SwitchToFirstPerson(this IPerspectiveSwitcher control) =>
            SwitchPerspective<IFirstPersonView>(control);

        public static Option<IThirdPersonView> SwitchToThirdPerson(this IPerspectiveSwitcher control) =>
            SwitchPerspective<IThirdPersonView>(control);
    }
}
