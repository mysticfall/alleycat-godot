using System;
using LanguageExt;

namespace AlleyCat.Autowire
{
    public interface IDependencyProvider
    {
        HashSet<Type> Provides { get; }
    }
}
