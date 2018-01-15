using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IAutowireContext : IServiceProvider
    {
        void Register([NotNull] object instance);
    }
}
