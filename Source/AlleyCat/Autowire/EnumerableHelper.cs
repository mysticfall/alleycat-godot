using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    internal static class EnumerableHelper
    {
        private static readonly MethodInfo CastMethod =
            typeof(Enumerable).GetMethod("Cast", new[] {typeof(IEnumerable)});

        private static readonly MethodInfo OfTypeMethod =
            typeof(Enumerable).GetMethod("OfType", new[] {typeof(IEnumerable)});

        private static readonly MethodInfo EmptyMethod =
            typeof(Enumerable).GetMethod("Empty", new Type[0]);

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

        public static IEnumerable Empty(Type type)
        {
            Debug.Assert(EmptyMethod != null, "EmptyMethod != null");

            return (IEnumerable) EmptyMethod.MakeGenericMethod(type).Invoke(null, new object[0]);
        }

        public static IEnumerable Cast(IEnumerable instance, Type type)
        {
            Ensure.That(instance, nameof(instance)).IsNotNull();

            Debug.Assert(CastMethod != null, "CastMethod != null");

            return (IEnumerable) CastMethod.MakeGenericMethod(type).Invoke(null, new object[] {instance});
        }

        public static IEnumerable Concat(object source, object target, Type type)
        {
            Ensure.That(source, nameof(source)).IsNotNull();
            Ensure.That(target, nameof(target)).IsNotNull();

            Debug.Assert(ConcatMethod != null, "CastMethod != null");

            return (IEnumerable) ConcatMethod.MakeGenericMethod(type).Invoke(null, new[] {source, target});
        }

        public static IEnumerable OfType(object instance, Type type)
        {
            Ensure.That(instance, nameof(instance)).IsNotNull();

            Debug.Assert(OfTypeMethod != null, "OfTypeMethod != null");

            return (IEnumerable) OfTypeMethod.MakeGenericMethod(type).Invoke(null, new[] {instance});
        }
    }
}
