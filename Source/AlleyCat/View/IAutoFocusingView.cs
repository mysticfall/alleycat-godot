using EnsureThat;
using Godot;

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
        public static void EnableDof(this IAutoFocusingView view, bool enable = true)
        {
            Ensure.That(view, nameof(view)).IsNotNull();

            var env = view.Camera.GetWorld().Environment;

            if (env == null) return;

            env.DofBlurNearEnabled = enable;
            env.DofBlurFarEnabled = enable;
        }

        public static void DisableDof(this IAutoFocusingView view) => EnableDof(view, false);

        public static void SetFocalDistance(this IAutoFocusingView view, float distance)
        {
            Ensure.That(view, nameof(view)).IsNotNull();

            var env = view.Camera.GetWorld().Environment;

            if (env == null) return;

            var effective = Mathf.Max(0, distance);

            env.DofBlurNearEnabled = true;
            env.DofBlurFarEnabled = effective <= view.MaxDofDistance;

            var offset = view.FocusRange / 2f;

            env.DofBlurNearDistance = Mathf.Clamp(effective - offset, 0, view.MaxDofDistance);
            env.DofBlurFarDistance = effective + offset;
        }
    }
}
