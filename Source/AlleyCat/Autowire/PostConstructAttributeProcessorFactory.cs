using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public class PostConstructAttributeProcessorFactory : INodeProcessorFactory
    {
        public IEnumerable<INodeProcessor> Create(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            return type
                .GetMembers(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => (m.MemberType & MemberTypes.Method) != 0)
                .Select(m => (member: m, attribute: m.GetCustomAttribute<PostConstructAttribute>()))
                .Where((t, _) => t.attribute != null)
                .Select(t => new PostConstructAttributeProcessor((MethodInfo) t.member, t.attribute));
        }
    }
}
