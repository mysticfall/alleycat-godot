using System;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Attribute
{
    public interface IAttribute : INamed, IDescribable, IIconSource, IActivatable, IGameObject
    {
        float Value { get; }

        Option<IAttribute> Min { get; }

        Option<IAttribute> Max { get; }

        Option<IAttribute> Modifier { get; }

        IObservable<float> OnChange { get; }

        void Initialize(IAttributeHolder holder);
    }

    public static class AttributeExtensions
    {
        public static IObservable<float> OnRatioChange(this IAttribute attribute)
        {
            Ensure.That(attribute, nameof(attribute)).IsNotNull();

            var minValue = attribute.Max.Map(a => a.OnChange).ToObservable().Switch();
            var maxValue = attribute.Max.Map(a => a.OnChange).ToObservable().Switch();

            var ratio = attribute.OnChange.CombineLatest(minValue, maxValue, 
                (value, min, max) =>
                {
                    var denominator = max - min;

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    return denominator == 0 ? 0f : (value - min) / denominator;
                });

            return ratio;
        }
    }
}
