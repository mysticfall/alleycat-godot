using System;
using System.Collections.Generic;

namespace AlleyCat.Autowire
{
    public interface INodeProcessorFactory
    {
        IEnumerable<INodeProcessor> Create(Type type);
    }
}
