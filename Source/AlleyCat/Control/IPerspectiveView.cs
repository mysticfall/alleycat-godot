using System.Linq;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Physics;
using AlleyCat.View;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface IPerspectiveView : IView, ICharacterAware<IHumanoid>, IActivatable, IValidatable
    {
        bool AutoActivate { get; }
    }

    public static class PerspectiveViewExtensions
    {
        [CanBeNull]
        public static Intersection IntersectRay(
            [NotNull] this IPerspectiveView view,
            Vector3 to,
            object[] exclude = null,
            int collisionLayer = 2147483647)
        {
            Ensure.Any.IsNotNull(view, nameof(view));

            var character = view.Character;

            if (character == null)
            {
                return null;
            }

            var world = view.Camera.GetWorld();

            var defaultFilter = new object[] {character};
            var filter = exclude != null ? exclude.Concat(defaultFilter) : defaultFilter;

            return world.IntersectRay(character.Vision.Viewpoint, to, filter.ToArray(), collisionLayer);
        }
    }
}
