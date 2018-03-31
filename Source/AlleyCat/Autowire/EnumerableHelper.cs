using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AlleyCat.Autowire
{
    internal static class EnumerableHelper
    {
        private static readonly MethodInfo CastMethod =
            typeof(Enumerable).GetMethod("Cast", new[] {typeof(IEnumerable)});

        private static readonly MethodInfo AnyMethod =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == "Any")
                .Where(m =>
                {
                    var p = m.GetParameters();

                    return p.Length == 1 &&
                           p[0].ParameterType.IsGenericType &&
                           p[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                }).Single();

        private static readonly MethodInfo ConcatMethod =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == "Concat")
                .Where(m =>
                {
                    var p = m.GetParameters();

                    return p.Length == 2 &&
                           p[0].ParameterType.IsGenericType &&
                           p[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                           p[1].ParameterType.IsGenericType &&
                           p[1].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                }).Single();

        public static bool Any(object instance, Type type) =>
            (bool) AnyMethod.MakeGenericMethod(type).Invoke(null, new[] {instance});


        public static IEnumerable Cast(object instance, Type type) =>
            (IEnumerable) CastMethod.MakeGenericMethod(type).Invoke(null, new[] {instance});

        public static IEnumerable Concat(object source, object target, Type type) =>
            (IEnumerable) ConcatMethod.MakeGenericMethod(type).Invoke(null, new[] {source, target});
    }
}
