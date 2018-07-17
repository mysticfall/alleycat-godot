using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.View
{
    public interface IAutoFocusingView : IView
    {
        float MaxDofDistance { get; set; }

        float FocusRange { get; set; }

        float FocusSpeed { get; set; }
    }

    public static class ViewExtensions
    {
        public static void EnableDof([NotNull] this IAutoFocusingView view, bool enable = true)
        {
            Ensure.Any.IsNotNull(view, nameof(view));

            var env = view.Camera.GetWorld().Environment;

            if (env == null) return;

            env.DofBlurNearEnabled = enable;
            env.DofBlurFarEnabled = enable;
        }

        public static void DisableDof([NotNull] this IAutoFocusingView view) => EnableDof(view, false);

        public static void SetFocalDistance([NotNull] this IAutoFocusingView view, float distance)
        {
            Ensure.Any.IsNotNull(view, nameof(view));

            var env = view.Camera.GetWorld().Environment;

            if (env == null) return;

            env.DofBlurNearEnabled = true;
            env.DofBlurFarEnabled = distance <= view.MaxDofDistance;

            var offset = view.FocusRange / 2f;

            env.DofBlurNearDistance = Mathf.Clamp(distance - offset, 0, view.MaxDofDistance);
            env.DofBlurFarDistance = distance + offset;
        }
    }
}
