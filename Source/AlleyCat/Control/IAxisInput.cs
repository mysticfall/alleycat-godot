using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Control.Generic;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Control
{
    public interface IAxisInput : IInput<float>
    {
        float Sensitivity { get; set; }

        Option<Curve> Curve { get; set; }

        float DeadZone { get; set; }

        bool Interpolate { get; set; }

        float WindowSize { get; set; }

        float WindowShift { get; set; }
    }

    public static class AxisInputExtensions
    {
        public static Option<IAxisInput> FindAxis(this InputBindings bindings, string key = "Value")
        {
            Ensure.That(bindings, nameof(bindings)).IsNotNull();
            Ensure.That(key, nameof(key)).IsNotNull();

            return bindings.TryGetValue(key).OfType<IAxisInput>().HeadOrNone();
        }

        public static Option<IObservable<Vector2>> AsVector2Input(
            this InputBindings bindings, string xKey = "X", string yKey = "Y")
        {
            Ensure.That(bindings, nameof(bindings)).IsNotNull();
            Ensure.That(xKey, nameof(xKey)).IsNotNull();
            Ensure.That(yKey, nameof(yKey)).IsNotNull();

            return from xAxis in FindAxis(bindings, xKey)
                from yAxis in FindAxis(bindings, yKey)
                select xAxis.CombineLatest(yAxis, (x, y) => new Vector2(x, y));
        }
    }
}
