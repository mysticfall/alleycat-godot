using System;

namespace AlleyCat.Common
{
    public struct Range<T> where T : IComparable<T>
    {
        public T Min { get; }

        public T Max { get; }

        public Range(T min, T max)
        {
            Min = min;
            Max = max.CompareTo(min) > 0 ? max : min;
        }

        public T Clamp(T value)
        {
            var v = value.CompareTo(Min) > 0 ? value : Min;

            return v.CompareTo(Max) < 0 ? v : Max;
        }
    }
}
