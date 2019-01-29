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
        public static Option<IAxisInput> FindAxis(this IInputBindings bindings, string key = "Value")
        {
            Ensure.That(bindings, nameof(bindings)).IsNotNull();

            return bindings.Inputs.Find(key).OfType<IAxisInput>().HeadOrNone();
        }

        public static Option<IObservable<Vector2>> AsVector2Input(
            this IInputBindings bindings, string xKey = "X", string yKey = "Y")
        {
            return
                from xAxis in FindAxis(bindings, xKey)
                from yAxis in FindAxis(bindings, yKey)
                select xAxis.StartWith(0).CombineLatest(yAxis.StartWith(0),
                    (x, y) => new Vector2(x, y));
        }
    }
}
