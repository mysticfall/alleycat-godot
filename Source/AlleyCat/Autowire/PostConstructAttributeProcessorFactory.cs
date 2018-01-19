using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class PostConstructAttributeProcessorFactory : IAttributeProcessorFactory
    {
        public IEnumerable<IAttributeProcessor> Create(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            return type
                .GetMembers(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => (m.MemberType & MemberTypes.Method) != 0)
                .Select(m => (m, m.GetCustomAttribute<PostConstructAttribute>()))
                .Where((t, _) => t.Item2 != null)
                .Select(t => new PostConstructAttributeProcessor((MethodInfo) t.Item1, t.Item2));
        }
    }
}
