using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public abstract class MemberAttributeProcessorFactory<T> : INodeProcessorFactory where T : System.Attribute
    {
        public IEnumerable<INodeProcessor> Create(Type type)
        {
            Ensure.That(type, nameof(type)).IsNotNull();

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

        protected abstract INodeProcessor CreateProcessor(MemberInfo member, T attribute);
    }
}
