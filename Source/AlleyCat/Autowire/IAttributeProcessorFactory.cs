using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface IAttributeProcessorFactory
    {
        [NotNull]
        IEnumerable<IAttributeProcessor> Create([NotNull] Type type);
    }
}
