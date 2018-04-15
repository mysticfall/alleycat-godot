using System;
using EnsureThat;

namespace AlleyCat.Common
{
    public struct Range<T> where T : IComparable<T>
    {
        public T Min { get; }

        public T Max { get; }

        public Range(T min, T max)
        {
            Ensure.Bool.IsTrue(
                max.CompareTo(min) > 0,
                nameof(min),
                opt => opt.WithMessage($"Argument '{nameof(max)}' must be greater than '{nameof(max)}'."));

            Min = min;
            Max = max;
        }

        public T Clamp(T value)
        {
            var v = value.CompareTo(Min) > 0 ? value : Min;

            return v.CompareTo(Max) < 0 ? v : Max;
        }
    }
}
