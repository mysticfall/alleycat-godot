using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class SingletonAttribute : InjectableAttribute
    {
        public SingletonAttribute([NotNull] params Type[] types) : base(types)
        {
        }
    }
}
