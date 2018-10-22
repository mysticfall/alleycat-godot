using System;

namespace AlleyCat.Autowire
{
    public class SingletonAttribute : InjectableAttribute
    {
        public SingletonAttribute(params Type[] types) : base(types)
        {
        }
    }
}
