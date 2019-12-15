using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Game
{
    [NonInjectable]
    public interface IGameNodeFactory : IServiceFactory, IServiceDefinitionProvider
    {
    }

    namespace Generic
    {
        public interface IGameNodeFactory<T> : IGameNodeFactory, Common.Generic.IServiceFactory<T>
            where T : IGameNode
        {
        }
    }
}
