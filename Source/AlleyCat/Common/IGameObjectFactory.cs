using AlleyCat.Autowire;
using LanguageExt;

namespace AlleyCat.Common
{
    public interface IGameObjectFactory : IServiceDefinitionProvider
    {
        Validation<string, object> Service { get; }
    }

    namespace Generic
    {
        public interface IGameObjectFactory<T> : IGameObjectFactory
        {
            new Validation<string, T> Service { get; }
        }
    }
}
