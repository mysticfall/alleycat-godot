using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public abstract class MemberAttributeProcessorFactory<T> : INodeProcessorFactory
        where T : Attribute
    {
        public IEnumerable<INodeProcessor> Create(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            return type
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => (m.MemberType & (MemberTypes.Property | MemberTypes.Field)) != 0)
                .Select(m => (m, m.GetCustomAttribute<T>()))
                .Where((t, _) => t.Item2 != null)
                .Select(t => CreateProcessor(t.Item1, t.Item2));
        }

        [NotNull]
        protected abstract INodeProcessor CreateProcessor(
            [NotNull] MemberInfo member, [NotNull] T attribute);
    }
}
