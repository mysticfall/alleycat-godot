using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public interface IDirectional
    {
        Vector3 Origin { get; }

        Vector3 Forward { get; }

        Vector3 Up { get; }

        Vector3 Right { get; }
    }

    public static class DirectionalExtensions
    {
        public static Transform GetTransform(this IDirectional directional)
        {
            Ensure.That(directional, nameof(directional)).IsNotNull();

            var basis = BasisExtensions.CreateFromAxes(
                directional.Right, directional.Up, directional.Forward * -1);

            return new Transform(basis, directional.Origin);
        }
    }
}
