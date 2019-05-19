using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Microsoft.Extensions.Caching.Memory;
using static LanguageExt.Prelude;
using Object = System.Object;

namespace AlleyCat.Autowire
{
    public static class TypeUtils
    {
        private static LanguageExt.HashSet<Type> _excludedTypes = HashSet(
            typeof(IDisposable),
            typeof(IEnumerable),
            typeof(Node),
            typeof(Object)
        );

        // ReSharper disable once StaticMemberInGenericType
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public static void RegisterExcludedType(Type type)
        {
            _excludedTypes = _excludedTypes.TryAdd(type);
        }

        public static IEnumerable<Type> FindInjectableTypes<T>() => FindInjectableTypes(typeof(T));

        public static IEnumerable<Type> FindInjectableTypes(Type type)
        {
            return Cache.GetOrCreate(type, _ =>
            {
                var attributeType = typeof(NonInjectableAttribute);

                return EnumerateInjectableTypes(type)
                    .Filter(i => !i.IsDefined(attributeType, false))
                    .Filter(i => !_excludedTypes.Contains(i))
                    .Freeze();
            });
        }

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
