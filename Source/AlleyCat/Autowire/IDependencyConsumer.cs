using System;
using LanguageExt;

namespace AlleyCat.Autowire
{
    public interface IDependencyConsumer
    {
        HashSet<Type> Requires { get; }
    }
}
