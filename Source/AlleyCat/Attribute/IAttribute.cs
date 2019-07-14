using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    public interface IAttribute : INamed, IDescribable, IIconSource, IActivatable, IGameObject
    {
        float Value { get; }

        Option<IAttribute> Min { get; }

        Option<IAttribute> Max { get; }

        Option<IAttribute> Modifier { get; }

        Map<string, IAttribute> Children { get; }

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

        public static Option<IAttribute> FindAttribute(this IAttribute attribute, string path, IAttributeHolder holder)
        {
            Ensure.That(attribute, nameof(attribute)).IsNotNull();
            Ensure.That(path, nameof(path)).IsNotNull();
            Ensure.That(holder, nameof(holder)).IsNotNull();

            Option<IAttribute> ResolvePath(IAttribute context, ISeq<string> segments) => segments.Match(
                () => None,
                key => context.Children.Find(key),
                (h, t) => context.Children.Find(h).Bind(a => ResolvePath(a, t)));

            Option<IAttribute> ResolveAbsolutePath(ISeq<string> segments) => segments.Match(
                () => None,
                key => holder.Attributes.TryGetValue(key),
                (h, t) => holder.Attributes.TryGetValue(h).Bind(c => ResolvePath(c, t)));

            var s = path.Split('/').SkipWhile(v => v == "." || v == "").ToSeq();

            return path.StartsWith("/") ? ResolveAbsolutePath(s) : ResolvePath(attribute, s);
        }
    }
}
