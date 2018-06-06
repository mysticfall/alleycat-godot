using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Character;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface IPlayerControl : ICharacterAware<IHumanoid>
    {
        IEnumerable<IPerspectiveView> Perspectives { get; }

        IPerspectiveView Perspective { get; set; }

        IObservable<IPerspectiveView> OnPerspectiveChange { get; }
    }

    public static class PlayerControlExtensions
    {
        [CanBeNull]
        public static T SwitchPerspective<T>([NotNull] this IPlayerControl control) where T : IPerspectiveView
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
        public static IFirstPersonView SwitchToFirstPerson([NotNull] this IPlayerControl control) =>
            SwitchPerspective<IFirstPersonView>(control);

        [CanBeNull]
        public static IThirdPersonView SwitchToThirdPerson([NotNull] this IPlayerControl control) =>
            SwitchPerspective<IThirdPersonView>(control);
    }
}
