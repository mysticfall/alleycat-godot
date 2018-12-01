using AlleyCat.Autowire;
using LanguageExt;

namespace AlleyCat.Game
{
    public interface IGameObjectFactory : IServiceDefinitionProvider
    {
        Validation<string, object> Service { get; }
    }

    namespace Generic
    {
        public interface IGameObjectFactory<T> : IGameObjectFactory where T : IGameObject
        {
            new Validation<string, T> Service { get; }
        }
    }
}
