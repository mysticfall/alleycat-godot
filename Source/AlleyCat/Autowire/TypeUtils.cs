using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace AlleyCat.Autowire
{
    public static class TypeUtils
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public static IEnumerable<Type> FindInjectableTypes<T>() => FindInjectableTypes(typeof(T));

        public static IEnumerable<Type> FindInjectableTypes(Type type) =>
            Cache.GetOrCreate(type, _ => EnumerateInjectableTypes(type));

        private static IEnumerable<Type> EnumerateInjectableTypes(Type type)
        {
            if (type == null)
            {
                yield break;
            }

            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            var current = type;

            while (current != null)
            {
                yield return current;

                current = current.BaseType;
            }
        }
    }
}
