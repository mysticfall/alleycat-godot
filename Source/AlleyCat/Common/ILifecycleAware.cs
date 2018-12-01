using System;
using AlleyCat.Autowire;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface ILifecycleAware : IInitializable, IDisposable
    {
    }
}
