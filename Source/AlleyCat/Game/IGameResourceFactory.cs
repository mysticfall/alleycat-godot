using AlleyCat.Common;
using AlleyCat.Common.Generic;

namespace AlleyCat.Game
{
    [Autowire.NonInjectable]
    public interface IGameResourceFactory : IServiceFactory
    {
    }

    namespace Generic
    {
        public interface IGameResourceFactory<T> : IServiceFactory<T> where T : IGameResource
        {
        }
    }
}
