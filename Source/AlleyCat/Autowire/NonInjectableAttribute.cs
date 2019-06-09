using System;

namespace AlleyCat.Autowire
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class NonInjectableAttribute : System.Attribute
    {
    }
}
