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
            Ensure.That(max, nameof(max)).IsGte(min);

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
