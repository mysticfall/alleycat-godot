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

            var declared = type
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.DeclaredOnly)
                .Where(m => (m.MemberType & (MemberTypes.Property | MemberTypes.Field)) != 0)
                .Select(m => (member: m, attribute: m.GetCustomAttribute<T>()))
                .Where((t, _) => t.attribute != null)
                .Select(t => CreateProcessor(t.member, t.attribute));

            var parent = type.BaseType;

            return parent == null ? declared : declared.Concat(Create(parent));
        }

        [NotNull]
        protected abstract INodeProcessor CreateProcessor([NotNull] MemberInfo member, [NotNull] T attribute);
    }
}
