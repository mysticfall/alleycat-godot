using System;
using EnsureThat;
using LanguageExt.TypeClasses;

namespace AlleyCat.Common
{
    public struct Range<T> where T : IComparable<T>
    {
        public T Min { get; }

        public T Max { get; }

        private readonly Num<T> _num;

        public Range(T min, T max, Num<T> num)
        {
            Ensure.That(num, nameof(num)).IsNotNull();

            Min = min;
            Max = max.CompareTo(min) > 0 ? max : min;

            _num = num;
        }

        public T Clamp(T value)
        {
            var v = value.CompareTo(Min) > 0 ? value : Min;

            return v.CompareTo(Max) < 0 ? v : Max;
        }

        public T Distance(T value)
        {
            var center = _num.Divide(_num.Plus(Min, Max), _num.FromFloat(2f));
            var diff = _num.Subtract(value, center);

            return _num.Product(diff, _num.Signum(diff));
        }

        public T Ratio(T value)
        {
            var center = _num.Divide(_num.Plus(Min, Max), _num.FromFloat(2f));
            var length = _num.Divide(_num.Subtract(Max, Min), _num.FromFloat(2f));
            var diff = _num.Subtract(value, center);

            return _num.Divide(diff, length);
        }

        public static Range<T> operator +(Range<T> left, Range<T> right)
        {
            var min = left._num.Plus(left.Min, right.Min);
            var max = left._num.Plus(left.Max, right.Max);

            return new Range<T>(min, max, left._num);
        }

        public override string ToString() => $"Range[Min = {Min}, Max = {Max}]";
    }
}
