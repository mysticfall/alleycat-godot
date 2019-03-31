using AlleyCat.Motion;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Sensor
{
    public interface IVision : ISense, IDirectional
    {
        Option<Vector3> LookTarget { get; set; }

        Vector3 Viewpoint { get; }

        Vector3 LineOfSight { get; }
    }

    public static class VisionExtensions
    {
        public static void LookAt(this IVision vision, Option<Vector3> target)
        {
            Ensure.That(vision, nameof(vision)).IsNotNull();

            vision.LookTarget = target;
        }
    }
}
