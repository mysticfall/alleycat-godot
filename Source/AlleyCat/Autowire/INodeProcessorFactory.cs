using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface INodeProcessorFactory
    {
        [NotNull]
        IEnumerable<INodeProcessor> Create([NotNull] Type type);
    }
}
