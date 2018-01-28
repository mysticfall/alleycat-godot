using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IDependencyConsumer
    {
        [NotNull]
        ISet<Type> Requires { get; }
    }
}
