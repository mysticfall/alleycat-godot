using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IAutowireContext : IServiceProvider
    {
        void Resolve([NotNull] object instance);
    }
}
