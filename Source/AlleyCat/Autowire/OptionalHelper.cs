using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Autowire
{
    internal static class OptionalHelper
    {
        private static readonly MethodInfo HeadOrNoneMethod =
            typeof(ListExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == "HeadOrNone")
                .Where(m =>
                {
                    var p = m.GetParameters();

                    return p.Length == 1 &&
                           p[0].ParameterType.IsGenericType &&
                           p[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                }).Single();

        public static IOptional HeadOrNone(object instance, Type type)
        {
            Ensure.That(instance, nameof(instance)).IsNotNull();
            Ensure.That(type, nameof(type)).IsNotNull();

            Debug.Assert(HeadOrNoneMethod != null, "HeadOrNoneMethod != null");

            return (IOptional) HeadOrNoneMethod.MakeGenericMethod(type).Invoke(null, new[] {instance});
        }
    }
}
