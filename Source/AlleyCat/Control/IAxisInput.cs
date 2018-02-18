using System;
using System.Reactive.Linq;
using AlleyCat.Control.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public interface IAxisInput : IInput<float>
    {
        float Sensitivity { get; set; }

        [CanBeNull]
        Curve Curve { get; set; }

        float DeadZone { get; set; }

        bool Interpolate { get; set; }

        float WindowSize { get; set; }

        float WindowShift { get; set; }
    }

    public static class AxisInputExtensions
    {
        [CanBeNull]
        public static IAxisInput GetAxis([NotNull] this InputBindings bindings, [NotNull] string key)
        {
            Ensure.Any.IsNotNull(bindings, nameof(bindings));
            Ensure.Any.IsNotNull(key, nameof(key));

            return bindings.ContainsKey(key) ? bindings[key] as IAxisInput : null;
        }

        [CanBeNull]
        public static IObservable<Vector2> AsVector2Input(
            [NotNull] this InputBindings bindings, [NotNull] string xKey = "X", string yKey = "Y")
        {
            Ensure.Any.IsNotNull(bindings, nameof(bindings));
            Ensure.Any.IsNotNull(xKey, nameof(xKey));
            Ensure.Any.IsNotNull(yKey, nameof(yKey));

            var xAxis = GetAxis(bindings, xKey);
            var yAxis = GetAxis(bindings, yKey);

            if (xAxis == null || yAxis == null)
            {
                return null;
            }

            return xAxis.CombineLatest(yAxis, (x, y) => new Vector2(x, y));
        }
    }
}
