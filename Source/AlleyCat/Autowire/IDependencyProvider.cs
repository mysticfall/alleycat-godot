using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IDependencyProvider
    {
        [NotNull]
        ISet<Type> Provides { get; }
    }
}
