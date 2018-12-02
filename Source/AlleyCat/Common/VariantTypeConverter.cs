using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Godot;

namespace AlleyCat.Common
{
    public class VariantTypeConverter : TypeConverter
    {
        public static Type[] VariantTypes => new[]
        {
            typeof(Vector2), typeof(Vector3), typeof(Rect2), typeof(Transform), typeof(Transform2D),
            typeof(Plane), typeof(Quat), typeof(Basis), typeof(AABB), typeof(Color), typeof(NodePath)
        };

        private static bool _installed;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string);

        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                return GD.Str2Var(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            VariantTypes.Any(t => t == destinationType);

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            value == null ? null : GD.Var2Str(value);

        public static void Install()
        {
            if (_installed) return;

            var converter = typeof(VariantTypeConverter);

            VariantTypes.Iter(t => TypeDescriptor.AddAttributes(t, new TypeConverterAttribute(converter)));

            _installed = true;
        }
    }
}
